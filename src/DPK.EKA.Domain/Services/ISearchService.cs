using DPK.EKA.Domain.Models;

namespace DPK.EKA.Domain.Services
{
    public interface ISearchService
    {
        Task UploadChunksAsync(IEnumerable<DocumentChunk> chunks);

        Task<List<DocumentChunk>> SearchAsync(float[] queryVector, string query);
    }
}
