using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Application.Tests.Activities;

public class GetActivitiesBySectionUseCaseTests
{
    [Fact]
    public void Execute_ReturnsOnlyActivitiesOfThatSection()
    {
        var repository = new FakeActivityRepository();
        var sectionId = Guid.NewGuid();
        var matching = Activity.Create(sectionId, "Tarea", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        var other = Activity.Create(Guid.NewGuid(), "Otra", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        repository.Save(matching);
        repository.Save(other);
        var useCase = new GetActivitiesBySectionUseCase(repository);

        var results = useCase.Execute(new GetActivitiesBySectionQuery(sectionId));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoActivities_ReturnsEmpty()
    {
        var useCase = new GetActivitiesBySectionUseCase(new FakeActivityRepository());

        var results = useCase.Execute(new GetActivitiesBySectionQuery(Guid.NewGuid()));

        Assert.Empty(results);
    }
}
