using DPK.EKA.Domain.Entities;

namespace DPK.EKA.Domain.Repositories
{
    public interface IConversationRepository
    {
        Task<Conversation> CreateConversationAsync(Conversation conversation);
    }
}
