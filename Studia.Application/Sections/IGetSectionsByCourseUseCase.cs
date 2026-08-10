namespace Studia.Application.Sections;

public interface IGetSectionsByCourseUseCase
{
    IReadOnlyCollection<SectionResult> Execute(GetSectionsByCourseQuery query);
}
