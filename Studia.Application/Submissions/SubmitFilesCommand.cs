namespace Studia.Application.Submissions;

public record SubmitFilesCommand(Guid ActivityId, Guid StudentId, IReadOnlyCollection<SubmittedFileInput> Files);
