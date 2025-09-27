using Microsoft.EntityFrameworkCore;

namespace AuthModule.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Entities.User> Users { get; set; }
    }
}
