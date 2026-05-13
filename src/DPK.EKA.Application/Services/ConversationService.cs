using DPK.EKA.Application.Interfaces;
using DPK.EKA.Domain.Entities;
using DPK.EKA.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DPK.EKA.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        public ConversationService([FromKeyedServices("MongoDb")] IConversationRepository conversationRepository) 
        { 
            _conversationRepository = conversationRepository;
        }
        public async Task<Conversation> CreateConversationAsync(string query, 
                                                                string answer, 
                                                                List<string> sources)
        {
            var conversation = new Conversation()
            {
                Id = Guid.NewGuid(),
                Query = query,
                Answer = answer,
                ApiVersion = "v1",
                CreatedAt = DateTime.UtcNow,
                Source = string.Join("|", sources)
            };

            return await _conversationRepository.CreateConversationAsync(conversation);
        }
    }
}
