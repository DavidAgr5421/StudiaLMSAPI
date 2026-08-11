using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

// StudentName es opcional por el mismo motivo que EnrollmentResult.StudentName: FromDomain(Submission)
// no tiene acceso al User -- harina de otro aggregate. Quien sí lo tenga
// (GetSubmissionsByActivityUseCase) lo completa después con una expresión "with".
public record SubmissionResult(
    Guid Id,
    Guid ActivityId,
    Guid StudentId,
    SubmissionStatus Status,
    DateTime SubmittedAtUtc,
    string? TextContent,
    IReadOnlyCollection<SubmittedFileResult> Files,
    int? Score,
    string? Feedback,
    string? StudentName = null)
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
