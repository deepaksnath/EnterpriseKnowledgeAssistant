using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;

namespace DPK.EKA.Application.Services
{
    public class RagService : IRagService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly ISearchService _searchService;
        private readonly IChatService _chatService;

        public RagService(
                          IEmbeddingService embeddingService,
                          ISearchService searchService,
                          IChatService chatService)
        {
            _embeddingService = embeddingService;
            _searchService = searchService;
            _chatService = chatService;
        }

        public async Task<RagResponse> GetAnswerAsync(string question)
        {
            // 1. Create embedding
            var queryVector = await _embeddingService.CreateEmbeddingAsync(question);

            // 2. Search (Top-K)
            var results = await _searchService.SearchAsync(queryVector.ToArray(), question);

            // 3. Build context
            var context = string.Join("\n", results.Select(r => r.Content));

            // 4. Call LLM
            var answer = await _chatService.GetChatResponseAsync(context, question);

            // 5. Extract sources
            var sources = results.Select(r => r.Source).Distinct().ToList();

            return new RagResponse(answer, sources);
        }
    }
}
