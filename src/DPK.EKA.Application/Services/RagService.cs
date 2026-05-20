using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DPK.EKA.Application.Services
{
    public class RagService : IRagService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly ISearchService _searchService;
        private readonly IChatService _chatService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<RagService> _logger;

        public RagService([FromKeyedServices("SemanticKernel")] IEmbeddingService embeddingService,
                          [FromKeyedServices("SemanticKernel")] IChatService chatService,
                          IConversationService conversationService,
                          ISearchService searchService,
                          ILogger<RagService> logger)
        {
            _conversationService = conversationService;
            _embeddingService = embeddingService;
            _searchService = searchService;
            _chatService = chatService;
            _logger = logger;
        }

        public async Task<RagResponse> GetAnswerAsync(string question)
        {
            _logger.LogInformation("Received question: {Question}", question);

            // 1. Create embedding
            var queryVector = await _embeddingService.CreateEmbeddingAsync(question);

            // 2. Search (Top-K)
            var results = await _searchService.SearchAsync(queryVector.ToArray(), question);

            // 3. Build context
            var context = string.Join("\n", results.Select(r => r.Content));

            // 4. Call LLM
            var answer = await _chatService.GetRagResponseAsync(context, question);

            // 5. Extract sources
            var sources = results.Select(r => r.Source).Distinct().ToList();

            // 6. Save conversation into database
            _ = await _conversationService.CreateConversationAsync(question, answer, sources);
            _logger.LogInformation("Conversation saved with question: {Question} and answer: {Answer}", question, answer);

            return new RagResponse(answer, sources);
        }
    }
}
