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
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Threading.RateLimiting;

namespace DPK.EKA.Api.Extensions
{
    public static class RegisterServiceDependencies
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            //Infrastructure Services
            builder.Services.RegisterInfrastructureServices(builder.Configuration);

            // Settings
            builder.Services.AddOptions<AzureAiSettings>()
                   .Bind(builder.Configuration.GetSection("AzureAiSettings"))
                   .ValidateDataAnnotations()
                   .ValidateOnStart();

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
                   .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
                   .AddSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
                                 name: "sql-db",
                                 tags: new[] { "ready" });

            // Global Exception Handling
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            
            // Services
            builder.Services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
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