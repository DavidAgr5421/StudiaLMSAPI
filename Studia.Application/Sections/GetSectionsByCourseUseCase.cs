namespace Studia.Application.Sections;

public class GetSectionsByCourseUseCase(ISectionRepository sectionRepository) : IGetSectionsByCourseUseCase
{
    public IReadOnlyCollection<SectionResult> Execute(GetSectionsByCourseQuery query) =>
        sectionRepository.GetByCourseId(query.CourseId)
            .Select(SectionResult.FromDomain)
            .ToList();
}
