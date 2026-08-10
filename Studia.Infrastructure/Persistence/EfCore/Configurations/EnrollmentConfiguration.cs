using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Enrollments;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CourseId).IsRequired();
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(e => e.RequestedAtUtc).IsRequired();
        builder.Property(e => e.DecidedAtUtc);

        builder.HasIndex(e => new { e.CourseId, e.StudentId });
    }
}
