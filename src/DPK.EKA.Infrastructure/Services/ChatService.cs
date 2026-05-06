using Azure.AI.OpenAI;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DPK.EKA.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deployment;

        public ChatService(AzureOpenAIClient client, IOptions<AzureAiSettings> settings)
        {
            _client = client;
            _deployment = settings.Value.ChatDeployment; // add this in config
        }

        public async Task<string> GetChatResponseAsync(string context, string question)
        {
            var chatClient = _client.GetChatClient(_deployment);

            var messages = new List<ChatMessage>
                           {
                               new SystemChatMessage(
                                   "You are a helpful assistant. Answer ONLY from the provided context. " +
                                   "If the answer is not in the context, say 'I don't know'."),

                               new UserChatMessage(
                                   $"Context:\n{context}\n\nQuestion:\n{question}")
                           };

            var response = await chatClient.CompleteChatAsync(messages);

            return response.Value.Content[0].Text;
        }
    }
}
