using Studia.Application.Cohorts;
using Studia.Application.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Activities;

public class GetActivitiesBySectionUseCase(
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICohortRepository cohortRepository) : IGetActivitiesBySectionUseCase
{
    public IReadOnlyCollection<ActivityResult> Execute(GetActivitiesBySectionQuery query)
    {
        var activities = activityRepository.GetBySectionId(query.SectionId);

        if (query.RequestingUserRole != Role.Estudiante)
            return activities.Select(ActivityResult.FromDomain).ToList();

        var section = sectionRepository.GetById(query.SectionId);
        if (section is null)
            return [];

        var myCohortIds = cohortRepository.GetByCourseId(section.CourseId)
            .Where(cohort => cohort.StudentIds.Contains(query.RequestingUserId))
            .Select(cohort => cohort.Id)
            .ToHashSet();

        // Si la sección misma está restringida a fichas ajenas al estudiante, tampoco
        // debería ver sus actividades por más que conozca el sectionId.
        if (section.CohortIds.Count > 0 && !section.CohortIds.Any(myCohortIds.Contains))
            return [];

        return activities
            .Where(activity => activity.CohortIds.Count == 0 || activity.CohortIds.Any(myCohortIds.Contains))
            .Select(ActivityResult.FromDomain)
            .ToList();
    }
}
