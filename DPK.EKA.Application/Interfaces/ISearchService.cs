using DPK.EKA.Domain;

namespace DPK.EKA.Application.Interfaces
{
    public interface ISearchService
    {
        Task UploadChunksAsync(IEnumerable<DocumentChunk> chunks);

        Task<List<DocumentChunk>> SearchAsync(float[] queryVector, string query);
    }
}
