namespace Studia.Application.Submissions;

public record EditTextSubmissionCommand(Guid SubmissionId, Guid StudentId, string TextContent);
