using GameLocalization.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameLocalization.Infrastructure.Data.Configurations
{
    public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Value)
                .IsRequired(false)
                .HasMaxLength(2500)
                .HasDefaultValue(string.Empty);

            builder.HasIndex(t => new { t.LocalizationKeyId, t.LanguageId })
                .IsUnique();

            builder.HasOne(t => t.Language)
                .WithMany(l => l.Translations)
                .HasForeignKey(t => t.LanguageId);

            builder.HasOne(t => t.LocalizationKey)
                .WithMany(k => k.Translations)
                .HasForeignKey(t => t.LocalizationKeyId);
        }
    }
}
