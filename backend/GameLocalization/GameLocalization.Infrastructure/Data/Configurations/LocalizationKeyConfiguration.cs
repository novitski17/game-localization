using GameLocalization.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameLocalization.Infrastructure.Data.Configurations
{
    public class LocalizationKeyConfiguration : IEntityTypeConfiguration<LocalizationKey>
    {
        public void Configure(EntityTypeBuilder<LocalizationKey> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Key)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(k => k.Key).IsUnique();

            builder.HasMany(k => k.Translations)
                .WithOne(t => t.LocalizationKey)
                .HasForeignKey(t => t.LocalizationKeyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
