using DPK.EKA.Domain.Entities;
using DPK.EKA.Domain.Repositories;

namespace DPK.EKA.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly RagDbContext _dbContext;

        public ConversationRepository(RagDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Conversation> CreateConversationAsync(Conversation conversation)
        {
            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();
            return conversation;
        }
    }
}
