using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public record ActivityFileResult(string FileName, string StorageKey, long SizeBytes)
{
    public static ActivityFileResult FromDomain(ActivityFile file) =>
        new(file.FileName, file.StorageKey, file.SizeBytes);
}
