using Studia.Domain.Sections;

namespace Studia.Application.Sections;

public record SectionResult(Guid Id, Guid CourseId, string Title, string DescriptionHtml, IReadOnlyCollection<Guid> CohortIds)
{
    public static SectionResult FromDomain(Section section) =>
        new(section.Id, section.CourseId, section.Title, section.DescriptionHtml, section.CohortIds);
}
