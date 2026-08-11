using Studia.Application.Submissions;

namespace Studia.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(string? rootPath = null)
    {
        _rootPath = rootPath ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(_rootPath);
    }

    public string Store(string fileName, byte[] content)
    {
        var storageKey = $"{Guid.NewGuid()}-{fileName}";
        File.WriteAllBytes(Path.Combine(_rootPath, storageKey), content);
        return storageKey;
    }

    public byte[]? Retrieve(string storageKey)
    {
        var path = Path.Combine(_rootPath, storageKey);
        return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }
}
