using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Auth;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("revoked_tokens");

        // Jti es naturalmente único (es un Guid generado por JwtTokenService) -- no hace
        // falta un Id separado, es la clave primaria misma.
        builder.HasKey(r => r.Jti);
        builder.Property(r => r.Jti).HasMaxLength(64);
        builder.Property(r => r.ExpiresAtUtc).IsRequired();
    }
}
