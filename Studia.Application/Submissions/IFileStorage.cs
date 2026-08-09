namespace Studia.Application.Submissions;

public interface IFileStorage
{
    string Store(string fileName, byte[] content);
}
