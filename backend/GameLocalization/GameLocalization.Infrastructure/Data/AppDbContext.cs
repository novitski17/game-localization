using GameLocalization.Core.Domain.Entities;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
