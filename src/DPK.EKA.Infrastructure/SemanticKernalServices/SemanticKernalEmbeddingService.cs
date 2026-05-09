using DPK.EKA.Domain.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace DPK.EKA.Infrastructure.SemanticKernalServices
{
    public class SemanticKernelEmbeddingService : IEmbeddingService
    {
        private readonly ITextEmbeddingGenerationService _embeddingService;

        public SemanticKernelEmbeddingService(Kernel kernel)
        {
            _embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        }

        public async Task<ReadOnlyMemory<float>> CreateEmbeddingAsync(string text)
        {
            return await _embeddingService.GenerateEmbeddingAsync(text);
        }
    }
}
