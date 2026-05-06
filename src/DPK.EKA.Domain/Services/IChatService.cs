namespace DPK.EKA.Domain.Services
{
    public interface IChatService
    {
        Task<string> GetChatResponseAsync(string context, string question);
    }
}
