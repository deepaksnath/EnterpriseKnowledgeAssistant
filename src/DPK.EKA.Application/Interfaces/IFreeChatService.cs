using DPK.EKA.Application.Models;

namespace DPK.EKA.Application.Interfaces
{
    public interface IFreeChatService
    {
        Task<RagResponse> GetAnswerAsync(string question);
    }
}
