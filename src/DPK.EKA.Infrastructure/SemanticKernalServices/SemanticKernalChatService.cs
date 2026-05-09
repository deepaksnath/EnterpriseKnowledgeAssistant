using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DPK.EKA.Infrastructure.SemanticKernalServices
{
    public class SemanticKernelChatService : IChatService
    {
        private readonly IOptions<AzureAiSettings> _settings;
        private readonly Kernel _kernel;

        public SemanticKernelChatService(Kernel kernel, IOptions<AzureAiSettings> settings)
        {
            _kernel = kernel;
            _settings = settings;
        }

        public async Task<string> GetChatResponseAsync(string context, string question)
        {
            string prompt = """
                            {{$chatCustomizationMessage}}

                            If the answer is not in the context, ONLY say:
                            {{$outOfContextReply}}

                            Context:
                            {{$chatContext}}

                            Question:
                            {{$chatQuestion}}
                            """;

            var settings = new OpenAIPromptExecutionSettings
            {
                // MaxTokens = _settings.Value.ChatMaxTokens,
                Temperature = _settings.Value.ChatTemperature,
                TopP = _settings.Value.ChatTopP
            };

            var arguments = new KernelArguments(settings)
            {
                ["chatCustomizationMessage"] = _settings.Value.ChatCustomizationMessage,
                ["outOfContextReply"] = _settings.Value.OutOfContextReply,
                ["chatContext"] = context,
                ["chatQuestion"] = question
            };

            var result = await _kernel.InvokePromptAsync(prompt, arguments);

            return result.ToString();
        }

        //public async Task<string> GetChatResponseAsync1(string context, string question)
        //{
        //    string prompt = $"{_settings.Value.ChatCustomizationMessage}" +
        //                    $"\n Answer ONLY from the provided context." +
        //                    $"\n If the answer is not in the context," +
        //                    $"\n reply exactly with: " +
        //                    $"\n'{_settings.Value.OutOfContextReply}'.";
                        
        //    var options = new ChatOptions
        //    {
        //        Temperature = _settings.Value.ChatTemperature,
        //        FrequencyPenalty = _settings.Value.ChatFrequencyPenalty,
        //        PresencePenalty = _settings.Value.ChatPresencePenalty,
        //        TopP = _settings.Value.ChatTopP
        //    };

        //    var result = await _chatClient.GetResponseAsync(prompt, options);

        //    return result.ToString();
        //}
    }
}
