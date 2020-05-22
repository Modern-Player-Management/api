using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<DbFile> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(m => m.TeamId);
        }
    }
}