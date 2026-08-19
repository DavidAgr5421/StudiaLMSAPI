using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Submissions;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ActivityId).IsRequired();
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.GroupId);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(s => s.SubmittedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc);
        builder.Property(s => s.TextContent).HasColumnType("text");
        builder.Property(s => s.Score);
        builder.Property(s => s.Feedback).HasColumnType("text");

        // SubmittedFile es un Value Object sin identidad propia -- se guarda como columna
        // jsonb en vez de una tabla aparte. Igual que StudentIds, mapea al campo privado
        // _files. La propiedad pública Files es solo un wrapper de lectura calculado a
        // partir de _files -- no es una segunda relación, así que hay que decirle a EF
        // explícitamente que la ignore, o la confunde con otra navegación.
        builder.Ignore(s => s.Files);

        builder.OwnsMany<SubmittedFile>("_files", files =>
        {
            files.ToJson();
            files.Property(f => f.FileName).HasMaxLength(260);
            files.Property(f => f.StorageKey).HasMaxLength(500);
            files.Property(f => f.SizeBytes);
        });
    }
}
