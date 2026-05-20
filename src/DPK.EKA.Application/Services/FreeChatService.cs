using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DPK.EKA.Application.Services
{
    public class FreeChatService : IFreeChatService
    {
        private readonly IChatService _chatService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<FreeChatService> _logger;

        public FreeChatService([FromKeyedServices("SemanticKernel")] IChatService chatService,
                               IConversationService conversationService,
                               ILogger<FreeChatService> logger)
        {
            _conversationService = conversationService;
            _chatService = chatService;
            _logger = logger;
        }

        public async Task<RagResponse> GetAnswerAsync(string question)
        {
            _logger.LogInformation("Received question: {Question}", question);

            var answer = await _chatService.GetChatResponseAsync(question);

            _ = await _conversationService.CreateConversationAsync(question, answer, new());
            _logger.LogInformation("Conversation saved with question: {Question} and answer: {Answer}", question, answer);

            return new RagResponse(answer, new());
        }
    }
}
