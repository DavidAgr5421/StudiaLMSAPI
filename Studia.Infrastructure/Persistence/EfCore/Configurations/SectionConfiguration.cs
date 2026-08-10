using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Domain.Sections;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("sections");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CourseId).IsRequired();
        builder.Property(s => s.Title).HasMaxLength(150).IsRequired();
        builder.Property(s => s.DescriptionHtml).HasColumnType("text").IsRequired();
    }
}
