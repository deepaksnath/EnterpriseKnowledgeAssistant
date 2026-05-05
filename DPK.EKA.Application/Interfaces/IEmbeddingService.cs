namespace DPK.EKA.Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<ReadOnlyMemory<float>> CreateEmbeddingAsync(string text);
    }
}
