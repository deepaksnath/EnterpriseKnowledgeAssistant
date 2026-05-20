using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPK.EKA.Application.Extensions
{
    public static class RegisterAppServices
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services,
                                                                     IConfiguration configuration)
        {
            // Services
            services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();
            services.AddScoped<IRagService, RagService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IFreeChatService, FreeChatService>();

            return services;
        }
    }
}
