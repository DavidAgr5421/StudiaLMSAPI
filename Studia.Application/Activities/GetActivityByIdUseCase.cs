using Studia.Application.Courses;
using Studia.Application.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Users;

namespace Studia.Application.Activities;

public class GetActivityByIdUseCase(
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository) : IGetActivityByIdUseCase
{
    public ActivityResult? Execute(GetActivityByIdQuery query)
    {
        var activity = activityRepository.GetById(query.ActivityId);
        if (activity is null) return null;

        var section = sectionRepository.GetById(activity.SectionId);
        var course = section is null ? null : courseRepository.GetById(section.CourseId);

        // Oculto: se comporta como si no existiera para cualquiera que no sea el
        // profesor dueño del curso (o un admin) -- 404, no 403, para no confirmar
        // siquiera que la actividad existe.
        if (activity.Status == ActivityStatus.Oculto)
        {
            var isOwner = query.RequestingUserRole == Role.Administrador ||
                (course is not null && course.ProfesorId == query.RequestingUserId);

            if (!isOwner) return null;
        }

        return ActivityResult.FromDomain(activity) with { CourseId = course?.Id };
    }
}
