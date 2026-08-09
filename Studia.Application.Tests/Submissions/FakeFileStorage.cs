using Studia.Application.Submissions;

namespace Studia.Application.Tests.Submissions;

public class FakeFileStorage : IFileStorage
{
    public List<(string FileName, byte[] Content)> StoredFiles { get; } = [];

    public string Store(string fileName, byte[] content)
    {
        StoredFiles.Add((fileName, content));
        return $"fake-key-{StoredFiles.Count}";
    }
}
