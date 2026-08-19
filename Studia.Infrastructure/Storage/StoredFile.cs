namespace Studia.Infrastructure.Storage;

// Contraparte de persistencia de IFileStorage -- no es un concepto de dominio, es un
// detalle técnico de cómo se guardan los bytes (comprimidos) de cualquier archivo
// subido (material de apoyo, entregas, portada de curso).
public class StoredFile
{
    public string StorageKey { get; init; } = null!;
    public byte[] CompressedContent { get; init; } = null!;
    public long OriginalSizeBytes { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
