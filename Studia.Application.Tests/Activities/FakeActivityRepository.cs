using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Application.Tests.Activities;

public class FakeActivityRepository : IActivityRepository
{
    private readonly Dictionary<Guid, Activity> _activities = new();

    public IReadOnlyCollection<Activity> SavedActivities => _activities.Values.ToList();

    public void Save(Activity activity) => _activities[activity.Id] = activity;

    public Activity? GetById(Guid id) => _activities.GetValueOrDefault(id);
}
