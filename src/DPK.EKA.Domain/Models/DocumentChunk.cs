namespace DPK.EKA.Domain.Models
{
    public class DocumentChunk
    {
        public string Id { get; set; } = default!;
        public string Content { get; set; } = default!;
        public float[] ContentVector { get; set; } = default!;
        public string Source { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}
