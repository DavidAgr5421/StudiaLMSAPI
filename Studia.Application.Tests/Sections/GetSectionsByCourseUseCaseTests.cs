using Studia.Application.Sections;
using Studia.Domain.Sections;

namespace Studia.Application.Tests.Sections;

public class GetSectionsByCourseUseCaseTests
{
    [Fact]
    public void Execute_ReturnsOnlySectionsOfThatCourse()
    {
        var repository = new FakeSectionRepository();
        var courseId = Guid.NewGuid();
        var matching = Section.Create(courseId, "Semana 1", "");
        var other = Section.Create(Guid.NewGuid(), "Otra", "");
        repository.Save(matching);
        repository.Save(other);
        var useCase = new GetSectionsByCourseUseCase(repository);

        var results = useCase.Execute(new GetSectionsByCourseQuery(courseId));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoSections_ReturnsEmpty()
    {
        var useCase = new GetSectionsByCourseUseCase(new FakeSectionRepository());

        var results = useCase.Execute(new GetSectionsByCourseQuery(Guid.NewGuid()));

        Assert.Empty(results);
    }
}
