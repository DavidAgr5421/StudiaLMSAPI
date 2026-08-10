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
    }
}
