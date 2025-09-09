using Microsoft.EntityFrameworkCore;
using Prueba.Domain.Entities;

namespace Prueba.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<LoginLog> LoginLogs { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginLog>(b =>
            {
                b.ToTable("login_log");
                b.HasKey(x => x.Id);
                b.Property(x => x.Username)
                    .HasColumnName("username")
                    .HasMaxLength(200)
                    .IsRequired();
                b.Property(x => x.LoginTime)
                    .HasColumnName("login_time")
                    .IsRequired();
                b.Property(x => x.AccessToken)
                    .HasColumnName("access_token")
                    .HasColumnType("nvarchar(max)");
            });
        }
    }
}
