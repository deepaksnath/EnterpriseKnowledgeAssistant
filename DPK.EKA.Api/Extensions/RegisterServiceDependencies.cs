using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Application.Services;
using DPK.EKA.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace DPK.EKA.Api.Extensions
{
    public static class RegisterServiceDependencies
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // settings
            var config = new ConfigurationBuilder()
                                              .SetBasePath(Directory.GetCurrentDirectory())
                                              .AddJsonFile("appsettings.json", optional: false)
                                              .Build();

            services.AddOptions<AzureAiSettings>()
                                    .Bind(config.GetSection("AzureAiSettings"))
                                    .ValidateDataAnnotations()
                                    .ValidateOnStart();

            // Azure clients
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

            // services
            services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IRagService, RagService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "RAG Knowledge API",
                    Version = "v1"
                });
            });

            return services;
        }
    }
}