using System.ComponentModel.DataAnnotations;

namespace DPK.EKA.Application.Models
{
    public class AzureAiSettings
    {
        public const string SectionName = "AzureOpenAiSettings";

        [Required, Url]
        public string AzureOpenAiEndpoint { get; set; } = string.Empty;

        [Required]
        public string AzureOpenAiApiKey { get; set; } = string.Empty;

        [Required]
        public string AzureOpenAiChatDeployment { get; set; } = string.Empty;

        [Required]
        public string AzureOpenAiEmbeddingDeployment { get; set; } = string.Empty;

        [Required]
        public string SearchApiKey { get; set; } = string.Empty;

        [Required, Url]
        public string SearchEndpoint { get; set; } = string.Empty;

        [Required]
        public string SearchIndexName { get; set; } = string.Empty;

        public int SearchSize { get; set; } 

        public int SearchKNC { get; set; }

        [Required]
        public int ChatMaxTokens { get; set; }

        [Required]
        public float ChatTemperature { get; set; }

        [Required]
        public float ChatFrequencyPenalty { get; set; }

        [Required]
        public float ChatPresencePenalty { get; set; }

        [Required]
        public float ChatTopP { get; set; }

        [Required]
        public string ChatCustomizationMessage { get; set; } = string.Empty;

        [Required]
        public string OutOfContextReply { get; set; } = string.Empty;
    }
}
