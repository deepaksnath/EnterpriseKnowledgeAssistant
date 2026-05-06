using DPK.EKA.Domain.Entities;

namespace DPK.EKA.Application.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> CreateConversationAsync(string query, string answer, List<string> sources);
    }
}
