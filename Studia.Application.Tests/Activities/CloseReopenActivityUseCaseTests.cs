using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Application.Tests.Activities;

public class CloseReopenActivityUseCaseTests
{
    [Fact]
    public void Close_BlocksSubmissions()
    {
        var activities = new FakeActivityRepository();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activities.Save(activity);
        var useCase = new CloseActivityUseCase(activities);

        var result = useCase.Execute(new CloseActivityCommand(activity.Id));

        Assert.True(result.IsManuallyClosed);
        Assert.False(result.AcceptsSubmissions);
    }

    [Fact]
    public void Close_WhenActivityDoesNotExist_Throws()
    {
        var useCase = new CloseActivityUseCase(new FakeActivityRepository());

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new CloseActivityCommand(Guid.NewGuid())));
    }

    [Fact]
    public void Reopen_AfterClose_RestoresSubmissions()
    {
        var activities = new FakeActivityRepository();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activity.Close();
        activities.Save(activity);
        var useCase = new ReopenActivityUseCase(activities);

        var result = useCase.Execute(new ReopenActivityCommand(activity.Id));

        Assert.False(result.IsManuallyClosed);
        Assert.True(result.AcceptsSubmissions);
    }
}
