using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public interface IActivityRepository
{
    void Save(Activity activity);

    Activity? GetById(Guid id);

    IReadOnlyCollection<Activity> GetBySectionId(Guid sectionId);
}
