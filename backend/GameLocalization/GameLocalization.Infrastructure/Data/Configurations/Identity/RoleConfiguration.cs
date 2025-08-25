using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GameLocalization.Core.Domain.Entities.Identity;

namespace GameLocalization.Infrastructure.Data.Configurations.Identity
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(64);
        }
    }
}
