namespace Studia.Application.Submissions;

public record GetSubmissionFileQuery(Guid SubmissionId, string StorageKey, Guid RequestingUserId, bool RequestingUserIsAdmin);
