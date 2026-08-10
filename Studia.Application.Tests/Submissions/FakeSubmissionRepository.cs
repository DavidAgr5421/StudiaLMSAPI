using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Application.Tests.Submissions;

public class FakeSubmissionRepository : ISubmissionRepository
{
    private readonly Dictionary<Guid, Submission> _submissions = new();

    public IReadOnlyCollection<Submission> SavedSubmissions => _submissions.Values.ToList();

    public void Save(Submission submission) => _submissions[submission.Id] = submission;

    public Submission? GetById(Guid id) => _submissions.GetValueOrDefault(id);

    public IReadOnlyCollection<Submission> GetByActivityId(Guid activityId) =>
        _submissions.Values.Where(s => s.ActivityId == activityId).ToList();

    public void DeleteByActivityId(Guid activityId)
    {
        foreach (var submission in GetByActivityId(activityId))
            _submissions.Remove(submission.Id);
    }
}
