using System.Collections.Concurrent;
using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Infrastructure.Persistence;

public class InMemoryActivityRepository : IActivityRepository
{
    private readonly ConcurrentDictionary<Guid, Activity> _activities = new();

    public void Save(Activity activity) => _activities[activity.Id] = activity;

    public Activity? GetById(Guid id) => _activities.GetValueOrDefault(id);

    public IReadOnlyCollection<Activity> GetBySectionId(Guid sectionId) =>
        _activities.Values.Where(a => a.SectionId == sectionId).ToList();
}
