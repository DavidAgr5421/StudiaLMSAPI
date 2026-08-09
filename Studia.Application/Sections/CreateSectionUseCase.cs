using Studia.Application.Courses;
using Studia.Domain.Courses;
using Studia.Domain.Sections;

namespace Studia.Application.Sections;

public class CreateSectionUseCase(
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository,
    IHtmlSanitizer htmlSanitizer) : ICreateSectionUseCase
{
    public SectionResult Execute(CreateSectionCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        if (course.Status != CourseStatus.Activo)
            throw new InvalidOperationException($"El curso '{course.Name}' no está activo.");

        var sanitizedDescription = htmlSanitizer.Sanitize(command.DescriptionHtml);

        var section = Section.Create(course.Id, command.Title, sanitizedDescription);

        sectionRepository.Save(section);

        return SectionResult.FromDomain(section);
    }
}
