using Azure.AI.OpenAI;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using DPK.EKA.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DPK.EKA.Infrastructure.Services
{
    public class AzureOpenAiChatService : IChatService
    {
        private readonly IOptions<AzureAiSettings> _settings;
        private readonly AzureOpenAIClient _client;
        private readonly PromptBuilder _promptBuilder;

        public AzureOpenAiChatService(PromptBuilder promptBuilder, 
                                      AzureOpenAIClient client, 
                                      IOptions<AzureAiSettings> settings)
        {
            _promptBuilder = promptBuilder;
            _settings = settings;
            _client = client;
        }

        public async Task<string> GetRagResponseAsync(string context, string question)
        {
            var chatClient = _client.GetChatClient(_settings.Value.AzureOpenAiChatDeployment);

            var (userPrompt, systemPrompt) = await _promptBuilder.BuildPrompt(context, question);

            var messages = new List<ChatMessage>
                           {
                               new SystemChatMessage(systemPrompt),

                               new UserChatMessage(userPrompt)
                           };

            var options = new ChatCompletionOptions()
                          {
                              Temperature = _settings.Value.ChatTemperature,
                              FrequencyPenalty = _settings.Value.ChatFrequencyPenalty,
                              PresencePenalty = _settings.Value.ChatPresencePenalty,
                              TopP = _settings.Value.ChatTopP
            };

            var response = await chatClient.CompleteChatAsync(messages, options);

            return response.Value.Content[0].Text;
        }
        public async Task<string> GetChatResponseAsync(string question)
        {
            var chatClient = _client.GetChatClient(_settings.Value.AzureOpenAiChatDeployment);

            var messages = new List<ChatMessage>
                           {
                               new SystemChatMessage(_settings.Value.ChatCustomizationMessage),

                               new UserChatMessage(question)
                           };

            var options = new ChatCompletionOptions()
            {
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
