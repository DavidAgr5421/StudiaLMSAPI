using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

public interface ISubmissionRepository
{
    void Save(Submission submission);

    Submission? GetById(Guid id);

    IReadOnlyCollection<Submission> GetByActivityId(Guid activityId);

    void DeleteByActivityId(Guid activityId);
}
