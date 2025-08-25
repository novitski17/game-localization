using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Language> Languages => Set<Language>();
        public DbSet<LocalizationKey> LocalizationKeys => Set<LocalizationKey>();
        public DbSet<Translation> Translations => Set<Translation>();

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
