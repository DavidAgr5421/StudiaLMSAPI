using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Auth;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(t => t.ExpiresAtUtc).IsRequired();
        builder.Property(t => t.UsedAtUtc);

        builder.HasIndex(t => t.TokenHash).IsUnique();
    }
}
