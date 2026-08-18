using Studia.Application.Sections;

namespace Studia.Application.Activities;

public class GetActivityByIdUseCase(IActivityRepository activityRepository, ISectionRepository sectionRepository) : IGetActivityByIdUseCase
{
    public ActivityResult? Execute(GetActivityByIdQuery query)
    {
        var activity = activityRepository.GetById(query.ActivityId);
        if (activity is null) return null;

        var section = sectionRepository.GetById(activity.SectionId);

        return ActivityResult.FromDomain(activity) with { CourseId = section?.CourseId };
    }
}
