namespace DPK.EKA.Application.Interfaces
{
    public interface IChatService
    {
        Task<string> GetChatResponseAsync(string context, string question);
    }
}
