using Azure.AI.OpenAI;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace DPK.EKA.Infrastructure.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deployment;

        public EmbeddingService(AzureOpenAIClient client, IOptions<AzureAiSettings> s)
        {
            _client = client;
            _deployment = s.Value.EmbeddingDeployment;
        }
        public async Task<ReadOnlyMemory<float>> CreateEmbeddingAsync( string text)
        {
            EmbeddingClient embeddingClient = _client.GetEmbeddingClient(_deployment);

            var result = await embeddingClient.GenerateEmbeddingsAsync(new List<string> { text });

            OpenAIEmbedding embedding = result.Value[0];

            return embedding.ToFloats();
        }
    }
}
