using DPK.EKA.Domain.Repositories;
using DPK.EKA.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            // Repositories
            services.AddScoped<IConversationRepository, ConversationRepository>();

            return services;
        }
    }
}
