using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Sections;

public class GetSectionsByCourseUseCase(
    ISectionRepository sectionRepository,
    ICohortRepository cohortRepository,
    ICourseRepository courseRepository) : IGetSectionsByCourseUseCase
{
    public IReadOnlyCollection<SectionResult> Execute(GetSectionsByCourseQuery query)
    {
        var sections = sectionRepository.GetByCourseId(query.CourseId);

        // Oculto: solo lo ve el profesor dueño del curso o un admin -- ni siquiera otro
        // profesor, y mucho menos un estudiante.
        var course = courseRepository.GetById(query.CourseId);
        var isOwner = query.RequestingUserRole == Role.Administrador ||
            (course is not null && course.ProfesorId == query.RequestingUserId);

        if (!isOwner)
            sections = sections.Where(section => section.Status != SectionStatus.Oculto).ToList();

        // Profesor/Administrador gestionan el curso: ven todas las secciones, sin
        // importar la ficha. El filtro por ficha es solo para el estudiante.
        if (query.RequestingUserRole != Role.Estudiante)
            return sections.Select(SectionResult.FromDomain).ToList();

        var myCohortIds = cohortRepository.GetByCourseId(query.CourseId)
            .Where(cohort => cohort.StudentIds.Contains(query.RequestingUserId))
            .Select(cohort => cohort.Id)
            .ToHashSet();

        return sections
            .Where(section => section.CohortIds.Count == 0 || section.CohortIds.Any(myCohortIds.Contains))
            .Select(SectionResult.FromDomain)
            .ToList();
    }
}
