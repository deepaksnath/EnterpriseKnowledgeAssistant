using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using UglyToad.PdfPig;

namespace DPK.EKA.Application.Services
{
    public class DocumentIngestionService : IDocumentIngestionService
    {
        private readonly IEmbeddingService _embedding;
        private readonly ISearchService _search;

        public DocumentIngestionService([FromKeyedServices("SemanticKernel")] IEmbeddingService e, 
                                        ISearchService s)
        {
            _embedding = e;
            _search = s;
        }

        public async Task<IngestionResult> ProcessAsync(Stream fileStream, string fileName)
        {
            var chunks = ExtractChunks(fileStream);

            var result = new List<DocumentChunk>();

            foreach (var chunk in chunks)
            {
                var vector = await _embedding.CreateEmbeddingAsync(chunk);

                result.Add(new DocumentChunk
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = chunk,
                    ContentVector = vector.ToArray(),
                    Source = fileName,
                    UploadedAt = DateTime.UtcNow
                });
            }

            await _search.UploadChunksAsync(result);

            return new IngestionResult(fileName, result.Count);
        }

        public static List<string> ExtractChunks(Stream pdfStream, int chunkSize = 500)
        {
            var chunks = new List<string>();

            using (var document = PdfDocument.Open(pdfStream))
            {
                foreach (var page in document.GetPages())
                {
                    var text = page.Text;

                    for (int i = 0; i < text.Length; i += chunkSize)
                    {
                        chunks.Add(text.Substring(i, Math.Min(chunkSize, text.Length - i)));
                    }
                }
            }

            return chunks;
        }
    }
}
