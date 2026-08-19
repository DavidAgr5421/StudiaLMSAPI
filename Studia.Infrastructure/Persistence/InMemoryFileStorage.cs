using System.Collections.Concurrent;
using Studia.Application.Submissions;

namespace Studia.Infrastructure.Persistence;

public class InMemoryFileStorage : IFileStorage
{
    private readonly ConcurrentDictionary<string, byte[]> _files = new();

    public string Store(string fileName, byte[] content)
    {
        var storageKey = $"{Guid.NewGuid()}-{fileName}";
        _files[storageKey] = content;
        return storageKey;
    }

    public byte[]? Retrieve(string storageKey) => _files.GetValueOrDefault(storageKey);
}
