using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Services;
using DPK.EKA.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DPK.EKA.Infrastructure.SemanticKernalServices
{
    public class SemanticKernelChatService : IChatService
    {
        private readonly IOptions<AzureAiSettings> _settings;
        private readonly Kernel _kernel;
        private readonly PromptBuilder _promptBuilder;

        public SemanticKernelChatService(PromptBuilder promptBuilder, 
                                         Kernel kernel, 
                                         IOptions<AzureAiSettings> settings)
        {
            _promptBuilder = promptBuilder;
            _kernel = kernel;
            _settings = settings;
        }

        public async Task<string> GetChatResponseAsync(string context, string question)
        {
            var (userPrompt, systemPrompt) = await _promptBuilder.BuildPrompt(context, question);

            string prompt = """
                            {{$systemPrompt}}
                                                        
                            {{$userPrompt}}
                            """;

            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = _settings.Value.ChatTemperature,
                TopP = _settings.Value.ChatTopP
            };

            var arguments = new KernelArguments(settings)
            {
                ["userPrompt"] = userPrompt,
                ["systemPrompt"] = systemPrompt
            };

            var result = await _kernel.InvokePromptAsync(prompt, arguments);

            return result.ToString();
        }
    }
}
