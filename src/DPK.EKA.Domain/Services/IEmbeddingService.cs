namespace DPK.EKA.Domain.Services
{ 
    public interface IEmbeddingService
    {
        Task<ReadOnlyMemory<float>> CreateEmbeddingAsync(string text);
    }
}
