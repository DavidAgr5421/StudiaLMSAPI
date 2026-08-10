using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Activities;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.SectionId).IsRequired();
        builder.Property(a => a.Title).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Description).HasColumnType("text").IsRequired();
        builder.Property(a => a.DueDateUtc).IsRequired();
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(a => a.MaxFiles);

        builder.Ignore(a => a.CohortIds);
        builder.PrimitiveCollection<List<Guid>>("_cohortIds").HasColumnName("CohortIds");

        // Igual que Submission.Files: value object sin identidad propia, se guarda como
        // columna jsonb en vez de una tabla aparte.
        builder.Ignore(a => a.Files);
        builder.OwnsMany<ActivityFile>("_files", files =>
        {
            files.ToJson();
            files.Property(f => f.FileName).HasMaxLength(260);
            files.Property(f => f.StorageKey).HasMaxLength(500);
            files.Property(f => f.SizeBytes);
        });
    }
}
