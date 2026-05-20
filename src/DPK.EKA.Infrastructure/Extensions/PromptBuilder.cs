using DPK.EKA.Application.Models;
using Microsoft.Extensions.Options;

namespace DPK.EKA.Infrastructure.Extensions
{
    public class PromptBuilder
    {
        private readonly IOptions<AzureAiSettings> _settings;
        public PromptBuilder(IOptions<AzureAiSettings> settings)
        {
            _settings = settings;
        }
        public async Task<(string, string)> BuildPrompt(string context, string question)
        {
            string systemPrompt = $"""
                                  {_settings.Value.ChatCustomizationMessage} 
                                  If the answer is not in the context,
                                  {_settings.Value.OutOfContextReply}
                                  """;

            string userPrompt = $"""
                                 Context:
                                 {context}

                                 Question:
                                 {question}
                                """;

            return await Task.FromResult((userPrompt, systemPrompt));
        }
    }
}
