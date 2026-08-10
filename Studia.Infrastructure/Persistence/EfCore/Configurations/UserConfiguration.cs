using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Users;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        // Email es un Value Object: se guarda como el string plano (ya normalizado),
        // pero el modelo de dominio nunca deja de trabajar con el tipo Email.
        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(320)
            .IsRequired();

        // Constraint real en la base, no solo la verificación en RegisterUserUseCase --
        // cierra la ventana de carrera entre dos registros concurrentes con el mismo email.
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Name).HasMaxLength(200);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(30).IsRequired();
    }
}
