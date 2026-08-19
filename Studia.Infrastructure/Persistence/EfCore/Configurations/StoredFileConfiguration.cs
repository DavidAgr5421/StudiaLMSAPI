using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Studia.Infrastructure.Storage;

namespace Studia.Infrastructure.Persistence.EfCore.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("stored_files");
        builder.HasKey(f => f.StorageKey);

        builder.Property(f => f.StorageKey).HasMaxLength(400).IsRequired();
        builder.Property(f => f.CompressedContent).IsRequired();
        builder.Property(f => f.OriginalSizeBytes).IsRequired();
        builder.Property(f => f.CreatedAtUtc).IsRequired();
    }
}
