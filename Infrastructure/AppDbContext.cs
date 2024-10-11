using MatMatShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatMatShop.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
    }
}
