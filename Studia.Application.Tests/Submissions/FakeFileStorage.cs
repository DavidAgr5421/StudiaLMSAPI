using Studia.Application.Submissions;

namespace Studia.Application.Tests.Submissions;

public class FakeFileStorage : IFileStorage
{
    private readonly Dictionary<string, byte[]> _filesByKey = new();

    public List<(string FileName, byte[] Content)> StoredFiles { get; } = [];

    public string Store(string fileName, byte[] content)
    {
        StoredFiles.Add((fileName, content));
        var storageKey = $"fake-key-{StoredFiles.Count}";
        _filesByKey[storageKey] = content;
        return storageKey;
    }

    public byte[]? Retrieve(string storageKey) => _filesByKey.GetValueOrDefault(storageKey);
}
