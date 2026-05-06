using DPK.EKA.Application.Models;

namespace DPK.EKA.Application.Interfaces
{
    public interface IRagService
    {
        Task<RagResponse> GetAnswerAsync(string question);
    }
}
