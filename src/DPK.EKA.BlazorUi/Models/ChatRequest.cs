namespace DPK.EKA.BlazorUi.Models
{
    public record ChatRequest(
        string question
    );

    public record ChatMessageDto(
        string Role,    // Usually "user" or "assistant" (and occasionally "system")
        string Content  // The text content of the message
    );
}
