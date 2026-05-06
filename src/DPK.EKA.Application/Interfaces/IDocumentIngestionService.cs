using DPK.EKA.Application.Models;

namespace DPK.EKA.Application.Interfaces
{
    public interface IDocumentIngestionService
    {
        Task<IngestionResult> ProcessAsync(Stream fileStream, string fileName);
    }
}
