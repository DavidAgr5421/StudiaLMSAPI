using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Domain.Courses;
using Studia.Domain.Sections;

namespace Studia.Application.Sections;

public class CreateSectionUseCase(
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository,
    ICohortRepository cohortRepository,
    IHtmlSanitizer htmlSanitizer) : ICreateSectionUseCase
{
    public SectionResult Execute(CreateSectionCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        if (course.Status != CourseStatus.Activo)
            throw new InvalidOperationException($"El curso '{course.Name}' no está activo.");

        var cohortIds = command.CohortIds ?? [];
        foreach (var cohortId in cohortIds)
        {
            var cohort = cohortRepository.GetById(cohortId)
                ?? throw new InvalidOperationException($"No existe una ficha con id '{cohortId}'.");

            if (cohort.CourseId != course.Id)
                throw new InvalidOperationException($"La ficha '{cohort.Name}' no pertenece a este curso.");
        }

        var sanitizedDescription = htmlSanitizer.Sanitize(command.DescriptionHtml);

        var section = Section.Create(course.Id, command.Title, sanitizedDescription, cohortIds, command.Status);

        sectionRepository.Save(section);

        return SectionResult.FromDomain(section);
    }
}
