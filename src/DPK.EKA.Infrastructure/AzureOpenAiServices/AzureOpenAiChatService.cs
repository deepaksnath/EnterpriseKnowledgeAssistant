using Azure.AI.OpenAI;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DPK.EKA.Infrastructure.Services
{
    public class AzureOpenAiChatService : IChatService
    {
        private readonly IOptions<AzureAiSettings> _settings;
        private readonly AzureOpenAIClient _client;

        public AzureOpenAiChatService(AzureOpenAIClient client, IOptions<AzureAiSettings> settings)
        {
            _settings = settings;
            _client = client;
        }

        public async Task<string> GetChatResponseAsync(string context, string question)
        {
            var chatClient = _client.GetChatClient(_settings.Value.ChatDeployment);

            var messages = new List<ChatMessage>
                           {
                               new SystemChatMessage(
                                   $"{_settings.Value.ChatCustomizationMessage}" +
                                   $"\n Answer ONLY from the provided context." +
                                   $"\n If the answer is not in the context," +
                                   $"\n reply exactly with: " +
                                   $"\n'{_settings.Value.OutOfContextReply}'."),

                               new UserChatMessage(
                                   $"Context:\n{context}\n\nQuestion:\n{question}")
                           };

            var options = new ChatCompletionOptions()
                          {
                              //MaxOutputTokenCount = _settings.Value.ChatMaxTokens,
                              Temperature = _settings.Value.ChatTemperature,
                              FrequencyPenalty = _settings.Value.ChatFrequencyPenalty,
                              PresencePenalty = _settings.Value.ChatPresencePenalty,
                              TopP = _settings.Value.ChatTopP
            };

            var response = await chatClient.CompleteChatAsync(messages, options);

            return response.Value.Content[0].Text;
        }
    }
}
