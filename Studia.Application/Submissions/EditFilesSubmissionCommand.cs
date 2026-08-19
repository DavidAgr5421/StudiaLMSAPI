namespace Studia.Application.Submissions;

public record EditFilesSubmissionCommand(Guid SubmissionId, Guid StudentId, IReadOnlyCollection<SubmittedFileInput> Files, string? Description);
