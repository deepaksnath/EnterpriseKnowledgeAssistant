using System.Text.Json.Serialization;

namespace DPK.EKA.BlazorUi.Models
{
    public record ChatResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; init; } = "";

        [JsonPropertyName("data")]
        public ChatDataPayload Data { get; init; } = new();
    }

    public record ChatDataPayload
    {
        [JsonPropertyName("answer")]
        public string Answer { get; init; } = "";

        [JsonPropertyName("sources")]
        public List<string> Sources { get; init; } = new();
    }
}
