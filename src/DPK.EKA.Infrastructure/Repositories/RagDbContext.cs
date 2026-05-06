using DPK.EKA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DPK.EKA.Infrastructure.Repositories
{
    public class RagDbContext : DbContext
    {
        public RagDbContext(DbContextOptions<RagDbContext> options) : base(options)
        {
        }
        public DbSet<Conversation> Conversations { get; set; }
    }
}
