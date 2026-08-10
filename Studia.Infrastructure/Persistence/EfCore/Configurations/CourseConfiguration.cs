using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Courses;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.EnrollmentMode).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(c => c.InvitationCode).HasMaxLength(16);
        builder.Property(c => c.ProfesorId).IsRequired();
        builder.HasIndex(c => c.ProfesorId);
    }
}
