using System.Reflection.Metadata;
using MatMatShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatMatShop.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<OrderDetail>()
                .HasOne(o => o.Order)
                .WithMany(c => c.OrderDetails)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<OrderDetail>()
                .HasOne(o => o.Image)
                .WithMany(c => c.OrderDetails)
                .HasForeignKey(c => c.ImageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
