using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Application.Services;
using DPK.EKA.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace DPK.EKA.Api.Extensions
{
    public static class RegisterServiceDependencies
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddControllers();

            //Global Exception Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

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

            // Swagger
            services.AddEndpointsApiExplorer();
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            //Api Versioning
            services.AddApiVersioning(options =>
                     {
                         options.DefaultApiVersion = new ApiVersion(1, 0);
                         options.AssumeDefaultVersionWhenUnspecified = true;
                         options.ReportApiVersions = true;
                         options.ApiVersionReader = new UrlSegmentApiVersionReader();
                     });

            services.AddVersionedApiExplorer(options =>
                     {
                         options.GroupNameFormat = "'v'VVV";
                         options.SubstituteApiVersionInUrl = true;
                     });

            // Rate limiting
            services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("SlidingWindowPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 3;
                    opt.SegmentsPerWindow = 3;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.RejectionStatusCode = 429;
            });

            return services;
        }
    }
}