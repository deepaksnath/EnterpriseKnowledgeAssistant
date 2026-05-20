namespace DPK.EKA.Domain.Services
{
    public interface IChatService
    {
        Task<string> GetChatResponseAsync(string question);
        Task<string> GetRagResponseAsync(string context, string question); 
    }
}
