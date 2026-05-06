using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Application.Services;
using DPK.EKA.Domain.Services;
using DPK.EKA.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using System.Threading.RateLimiting;
using DPK.EKA.Infrastructure.Extensions;

namespace DPK.EKA.Api.Extensions
{
    public static class RegisterServiceDependencies
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            //Infrastructure Services
            builder.Services.RegisterInfrastructureServices(builder.Configuration);

            // Settings
            builder.Services.AddOptions<AzureAiSettings>()
                    .Bind(builder.Configuration.GetSection("AzureAiSettings"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            builder.Services.AddControllers();

            //Logging - Serilog
            var logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Configuration)
                            .CreateLogger();
            builder.Logging.AddSerilog(logger);
            builder.Host.UseSerilog((ctx, conf) =>
            {
                conf.ReadFrom.Configuration(ctx.Configuration);
            });

            // Add Health Checks
            builder.Services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });
            
            // Global Exception Handling
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
                        
            // Azure Clients
            builder.Services.AddSingleton(sp =>
                     {
                         var s = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;
                         return new AzureOpenAIClient(new Uri(s.AzureOpenAiEndpoint),
                                                      new AzureKeyCredential(s.AzureOpenAiApiKey));
                     });

            builder.Services.AddSingleton(sp =>
                     {
                         var s = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;
                         return new SearchClient(new Uri(s.SearchEndpoint),
                                                 s.SearchIndexName,
                                                 new AzureKeyCredential(s.SearchApiKey));
                     });

            // Services
            builder.Services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
            builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IRagService, RagService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen();

            // Api Versioning
            builder.Services.AddApiVersioning(options =>
                     {
                         options.DefaultApiVersion = new ApiVersion(1, 0);
                         options.AssumeDefaultVersionWhenUnspecified = true;
                         options.ReportApiVersions = true;
                         options.ApiVersionReader = new UrlSegmentApiVersionReader();
                     });

            builder.Services.AddVersionedApiExplorer(options =>
                     {
                         options.GroupNameFormat = "'v'VVV";
                         options.SubstituteApiVersionInUrl = true;
                     });

            // Rate Limiting
            builder.Services.AddRateLimiter(options =>
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

            return builder;
        }
    }
}