using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Activities;

public class GetActivitiesBySectionUseCase(
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICohortRepository cohortRepository,
    ICourseRepository courseRepository) : IGetActivitiesBySectionUseCase
{
    public IReadOnlyCollection<ActivityResult> Execute(GetActivitiesBySectionQuery query)
    {
        var activities = activityRepository.GetBySectionId(query.SectionId);

        var section = sectionRepository.GetById(query.SectionId);
        if (section is null)
            return [];

        // Oculto: solo lo ve el profesor dueño del curso o un admin.
        var course = courseRepository.GetById(section.CourseId);
        var isOwner = query.RequestingUserRole == Role.Administrador ||
            (course is not null && course.ProfesorId == query.RequestingUserId);

        if (!isOwner)
        {
            // Si la sección misma está oculta, ninguna de sus actividades es visible,
            // sin importar el estado individual de cada una.
            if (section.Status == SectionStatus.Oculto)
                return [];

            activities = activities.Where(activity => activity.Status != ActivityStatus.Oculto).ToList();
        }

        if (query.RequestingUserRole != Role.Estudiante)
            return activities.Select(ActivityResult.FromDomain).ToList();

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
