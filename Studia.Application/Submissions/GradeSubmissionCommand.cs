namespace Studia.Application.Submissions;

public record GradeSubmissionCommand(Guid SubmissionId, int Score, string? Feedback);
