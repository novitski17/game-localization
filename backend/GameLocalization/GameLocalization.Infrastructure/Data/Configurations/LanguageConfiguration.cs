using GameLocalization.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameLocalization.Infrastructure.Data.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(l => l.Code)
                .IsUnique();

            builder.HasMany(k => k.Translations)
                .WithOne(t => t.Language)
                .HasForeignKey(t=> t.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
