using Studia.Application.Activities;
using Studia.Application.Tests.Submissions;
using Studia.Domain.Activities;

namespace Studia.Application.Tests.Activities;

public class GetActivityFileUseCaseTests
{
    [Fact]
    public void Execute_WithValidStorageKey_ReturnsFileContent()
    {
        var activities = new FakeActivityRepository();
        var fileStorage = new FakeFileStorage();
        var storageKey = fileStorage.Store("guia.pdf", [1, 2, 3]);
        var activity = Activity.Create(
            Guid.NewGuid(), "Tarea", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null,
            files: [ActivityFile.Create("guia.pdf", storageKey, 3)]);
        activities.Save(activity);

        var useCase = new GetActivityFileUseCase(activities, fileStorage);

        var result = useCase.Execute(new GetActivityFileQuery(activity.Id, storageKey));

        Assert.Equal("guia.pdf", result.FileName);
        Assert.Equal(new byte[] { 1, 2, 3 }, result.Content);
    }

    [Fact]
    public void Execute_WhenActivityDoesNotExist_Throws()
    {
        var useCase = new GetActivityFileUseCase(new FakeActivityRepository(), new FakeFileStorage());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new GetActivityFileQuery(Guid.NewGuid(), "some-key")));
    }

    [Fact]
    public void Execute_WhenStorageKeyDoesNotBelongToActivity_Throws()
    {
        var activities = new FakeActivityRepository();
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activities.Save(activity);

        var useCase = new GetActivityFileUseCase(activities, new FakeFileStorage());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new GetActivityFileQuery(activity.Id, "ajeno-key")));
    }
}
