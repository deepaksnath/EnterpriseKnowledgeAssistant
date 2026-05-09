using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Application.Services;
using DPK.EKA.Domain.Services;
using DPK.EKA.Infrastructure.Extensions;
using DPK.EKA.Infrastructure.SemanticKernalServices;
using DPK.EKA.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Serilog;
using System.Threading.RateLimiting;

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

            // Semantic Kernal Clients
            builder.Services.AddSingleton(sp =>
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

            // Services
            builder.Services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IEmbeddingService, SemanticKernelEmbeddingService>();
            builder.Services.AddScoped<ISearchService, AzureOpenAiSearchService>();
            builder.Services.AddScoped<IChatService, SemanticKernelChatService>();
            builder.Services.AddScoped<IRagService, RagService>();

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
                var policyName = "SlidingWindowPolicy";
                var permitLimit = builder.Configuration
                                         .GetSection("RateLimit")
                                         .GetValue<int>("PermitLimit", 3); 
                var window = builder.Configuration
                                    .GetSection("RateLimit")
                                    .GetValue<int>("Window", 3);
                options.AddSlidingWindowLimiter(policyName, opt =>
                       {
                           opt.Window = TimeSpan.FromMinutes(window);
                           opt.PermitLimit = permitLimit;
                           opt.SegmentsPerWindow = 3;
                           opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                       });

                options.RejectionStatusCode = 429;
            });

            return builder;
        }
    }
}