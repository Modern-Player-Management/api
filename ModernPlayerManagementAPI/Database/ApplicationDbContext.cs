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
    }
}