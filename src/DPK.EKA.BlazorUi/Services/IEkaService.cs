using DPK.EKA.BlazorUi.Models;

namespace DPK.EKA.BlazorUi.Services
{
    public interface IEkaService
    {
        Task<ChatResponse> SendMessageAsync(ChatRequest request, bool isRagChat = false);
        Task<bool> UploadPdfAsync(string fileName, Stream fileStream);
    }
}
