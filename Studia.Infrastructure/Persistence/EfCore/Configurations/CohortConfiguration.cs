using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Cohorts;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class CohortConfiguration : IEntityTypeConfiguration<Cohort>
{
    public void Configure(EntityTypeBuilder<Cohort> builder)
    {
        builder.ToTable("cohorts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CourseId).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();

        // StudentIds vive detrás de un campo privado (_studentIds) en el dominio -- EF Core
        // lo mapea directo al campo (Postgres array nativo uuid[]), sin necesidad de exponer
        // un setter público que rompería la encapsulación de Cohort.AssignStudent(). La
        // propiedad pública StudentIds es solo un wrapper de lectura, hay que ignorarla.
        builder.Ignore(c => c.StudentIds);

        builder.PrimitiveCollection<List<Guid>>("_studentIds")
            .HasColumnName("StudentIds");
    }
}
