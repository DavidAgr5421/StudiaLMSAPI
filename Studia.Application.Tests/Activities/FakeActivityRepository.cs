using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Application.Tests.Activities;

public class FakeActivityRepository : IActivityRepository
{
    private readonly Dictionary<Guid, Activity> _activities = new();

    public IReadOnlyCollection<Activity> SavedActivities => _activities.Values.ToList();

    public void Save(Activity activity) => _activities[activity.Id] = activity;

    public Activity? GetById(Guid id) => _activities.GetValueOrDefault(id);

    public IReadOnlyCollection<Activity> GetBySectionId(Guid sectionId) =>
        _activities.Values.Where(a => a.SectionId == sectionId).ToList();

    public IReadOnlyCollection<Activity> GetWithDueDateBetween(DateTime fromUtc, DateTime toUtc) =>
        _activities.Values.Where(a => a.DueDateUtc >= fromUtc && a.DueDateUtc <= toUtc).ToList();

    public void DeleteBySectionId(Guid sectionId)
    {
        foreach (var activity in GetBySectionId(sectionId))
            _activities.Remove(activity.Id);
    }
}
