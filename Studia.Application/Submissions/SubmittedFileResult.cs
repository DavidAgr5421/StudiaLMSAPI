using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

public record SubmittedFileResult(string FileName, string StorageKey, long SizeBytes)
{
    public static SubmittedFileResult FromDomain(SubmittedFile file) =>
        new(file.FileName, file.StorageKey, file.SizeBytes);
}
