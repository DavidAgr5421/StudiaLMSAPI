namespace Studia.Application.Activities;

public class GetActivitiesBySectionUseCase(IActivityRepository activityRepository) : IGetActivitiesBySectionUseCase
{
    public IReadOnlyCollection<ActivityResult> Execute(GetActivitiesBySectionQuery query) =>
        activityRepository.GetBySectionId(query.SectionId)
            .Select(ActivityResult.FromDomain)
            .ToList();
}
