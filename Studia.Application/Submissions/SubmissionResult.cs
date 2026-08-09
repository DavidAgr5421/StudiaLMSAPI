using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

public record SubmissionResult(
    Guid Id,
    Guid ActivityId,
    Guid StudentId,
    SubmissionStatus Status,
    DateTime SubmittedAtUtc,
    string? TextContent,
    IReadOnlyCollection<SubmittedFileResult> Files,
    int? Score,
    string? Feedback)
{
    public static SubmissionResult FromDomain(Submission submission) =>
        new(
            submission.Id,
            submission.ActivityId,
            submission.StudentId,
            submission.Status,
            submission.SubmittedAtUtc,
            submission.TextContent,
            submission.Files.Select(SubmittedFileResult.FromDomain).ToList(),
            submission.Score,
            submission.Feedback);
}
