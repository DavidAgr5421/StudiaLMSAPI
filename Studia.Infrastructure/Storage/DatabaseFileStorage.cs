using System.IO.Compression;
using Studia.Application.Submissions;
using Studia.Infrastructure.Persistence.EfCore;

namespace Studia.Infrastructure.Storage;

// Guarda los bytes comprimidos (gzip) en Postgres en vez del disco local -- en Render el
// disco del contenedor es efímero y se pierde en cada redeploy/reinicio, así que cualquier
// archivo guardado ahí desaparecía tarde o temprano. De paso, la compresión reduce el
// espacio que ocupan las entregas/material de apoyo en la base de datos.
public class DatabaseFileStorage(StudiaDbContext dbContext) : IFileStorage
{
    public string Store(string fileName, byte[] content)
    {
        var storageKey = $"{Guid.NewGuid()}-{fileName}";

        using var compressedStream = new MemoryStream();
        using (var gzip = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
            gzip.Write(content);

        dbContext.StoredFiles.Add(new StoredFile
        {
            StorageKey = storageKey,
            CompressedContent = compressedStream.ToArray(),
            OriginalSizeBytes = content.LongLength,
            CreatedAtUtc = DateTime.UtcNow
        });
        dbContext.SaveChanges();

        return storageKey;
    }

    public byte[]? Retrieve(string storageKey)
    {
        var stored = dbContext.StoredFiles.Find(storageKey);
        if (stored is null)
            return null;

        using var compressedStream = new MemoryStream(stored.CompressedContent);
        using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        gzip.CopyTo(decompressedStream);

        return decompressedStream.ToArray();
    }
}
