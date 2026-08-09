using System.Collections.Concurrent;
using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Infrastructure.Persistence;

public class InMemorySubmissionRepository : ISubmissionRepository
{
    private readonly ConcurrentDictionary<Guid, Submission> _submissions = new();

    public void Save(Submission submission) => _submissions[submission.Id] = submission;

    public Submission? GetById(Guid id) => _submissions.GetValueOrDefault(id);

    public IReadOnlyCollection<Submission> GetByActivityId(Guid activityId) =>
        _submissions.Values.Where(s => s.ActivityId == activityId).ToList();
}
