using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DPK.EKA.Application.Models;
using DPK.EKA.Domain.Repositories;
using DPK.EKA.Domain.Services;
using DPK.EKA.Infrastructure.Repositories;
using DPK.EKA.Infrastructure.SemanticKernalServices;
using DPK.EKA.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using MongoDB.Driver;

namespace DPK.EKA.Infrastructure.Extensions
{
    public static class RegisterInfraServices
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, 
                                                                        IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<RagDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Mongo DB
            services.AddScoped<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetSection("MongoDbSettings").GetValue<string>("ConnectionString");
                return new MongoClient(connectionString);
            });

            //Repository
            services.AddKeyedScoped<IConversationRepository, ConversationSqlRepository>("SqlServerDb");
            services.AddKeyedScoped<IConversationRepository, ConversationMongoDbRepository>("MongoDb");
            
            //Services
            services.AddKeyedScoped<IEmbeddingService, AzureOpenAiEmbeddingService>("AzureOpenAi");
            services.AddKeyedScoped<IEmbeddingService, SemanticKernelEmbeddingService>("SemanticKernel");
            services.AddKeyedScoped<IChatService, AzureOpenAiChatService>("AzureOpenAi");
            services.AddKeyedScoped<IChatService, SemanticKernelChatService>("SemanticKernel");
            services.AddScoped<ISearchService, AzureOpenAiSearchService>();

            // Azure Clients
            services.AddSingleton(sp =>
            {
                var s = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;
                return new AzureOpenAIClient(new Uri(s.AzureOpenAiEndpoint),
                                             new AzureKeyCredential(s.AzureOpenAiApiKey));
            });

            services.AddSingleton(sp =>
            {
                var s = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;
                return new SearchClient(new Uri(s.SearchEndpoint),
                                        s.SearchIndexName,
                                        new AzureKeyCredential(s.SearchApiKey));
            });

            // Semantic Kernal Clients
            services.AddSingleton(sp =>
            {
                var s = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;

                var kernelBuilder = Kernel.CreateBuilder();

                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: s.ChatDeployment,
                    endpoint: s.AzureOpenAiEndpoint,
                    apiKey: s.AzureOpenAiApiKey);

                kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
                    deploymentName: s.EmbeddingDeployment,
                    endpoint: s.AzureOpenAiEndpoint,
                    apiKey: s.AzureOpenAiApiKey);

                return kernelBuilder.Build();
            });

            return services;
        }
    }
}
