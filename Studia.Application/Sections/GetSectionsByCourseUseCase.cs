using Studia.Application.Cohorts;
using Studia.Domain.Users;

namespace Studia.Application.Sections;

public class GetSectionsByCourseUseCase(
    ISectionRepository sectionRepository,
    ICohortRepository cohortRepository) : IGetSectionsByCourseUseCase
{
    public IReadOnlyCollection<SectionResult> Execute(GetSectionsByCourseQuery query)
    {
        var sections = sectionRepository.GetByCourseId(query.CourseId);

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
