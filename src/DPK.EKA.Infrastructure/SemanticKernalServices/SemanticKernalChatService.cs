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
    }
}
