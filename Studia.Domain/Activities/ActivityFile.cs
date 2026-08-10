namespace Studia.Domain.Activities;

// Material de apoyo que el profesor adjunta a la actividad -- no confundir con
// Submissions.SubmittedFile, que es lo que entrega el estudiante.
public sealed record ActivityFile
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    public string FileName { get; }
    public string StorageKey { get; }
    public long SizeBytes { get; }

    private ActivityFile(string fileName, string storageKey, long sizeBytes)
    {
        FileName = fileName;
        StorageKey = storageKey;
        SizeBytes = sizeBytes;
    }

    public static ActivityFile Create(string fileName, string storageKey, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("La referencia de almacenamiento no puede estar vacía.", nameof(storageKey));

        if (sizeBytes <= 0)
            throw new ArgumentException("El tamaño del archivo debe ser mayor a cero.", nameof(sizeBytes));

        if (sizeBytes > MaxSizeBytes)
            throw new ArgumentException($"El archivo '{fileName}' supera el límite de {MaxSizeBytes / (1024 * 1024)}MB.", nameof(sizeBytes));

        return new ActivityFile(fileName.Trim(), storageKey, sizeBytes);
    }
}
