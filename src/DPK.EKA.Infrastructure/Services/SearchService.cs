using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DPK.EKA.Domain.Models;
using DPK.EKA.Domain.Services;

namespace DPK.EKA.Infrastructure.Services
{
    public class SearchService : ISearchService
    {
        private readonly SearchClient _search;

        public SearchService(SearchClient search) => _search = search;

        public async Task UploadChunksAsync(IEnumerable<DocumentChunk> chunks)
        {
            var docs = chunks.Select(c => new SearchDocument
            {
                ["id"] = c.Id,
                ["content"] = c.Content,
                ["contentVector"] = c.ContentVector,
                ["source"] = c.Source,
                ["uploadedAt"] = c.UploadedAt
            });

            await _search.UploadDocumentsAsync(docs);
        }

        public async Task<List<DocumentChunk>> SearchAsync(float[] queryVector, string query)
        {
            var results = new List<DocumentChunk>();

            var options = new SearchOptions
                          {
                              Size = 3,
                              Select = { "id", "content", "source" }
                          };

            options.VectorSearch = new VectorSearchOptions
                                   {
                                       Queries = {
                                                   new VectorizedQuery(queryVector)
                                                   {
                                                       KNearestNeighborsCount = 3,
                                                       Fields = { "contentVector" }
                                                   }
                                                 }
                                   };

            var response = await _search.SearchAsync<SearchDocument>(query, options);

            await foreach (var result in response.Value.GetResultsAsync())
            {
                var doc = result.Document;

                results.Add(new DocumentChunk
                {
                    Id = doc["id"]?.ToString(),
                    Content = doc["content"]?.ToString(),
                    Source = doc.ContainsKey("source") ? doc["source"]?.ToString() : null
                });
            }

            return results;
        }
    }
}
