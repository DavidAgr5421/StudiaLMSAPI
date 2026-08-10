using Studia.Application.Notifications;
using Studia.Domain.Notifications;

namespace Studia.Application.Tests.Notifications;

public class GetMyNotificationsUseCaseTests
{
    [Fact]
    public void Execute_ReturnsOnlyNotificationsOfThatUser()
    {
        var repository = new FakeNotificationRepository();
        var userId = Guid.NewGuid();
        var mine = Notification.Create(userId, NotificationType.NuevaActividad, "Título", "Mensaje");
        var ajena = Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", "Mensaje");
        repository.Save(mine);
        repository.Save(ajena);
        var useCase = new GetMyNotificationsUseCase(repository);

        var results = useCase.Execute(new GetMyNotificationsQuery(userId));

        var result = Assert.Single(results);
        Assert.Equal(mine.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoNotifications_ReturnsEmpty()
    {
        var useCase = new GetMyNotificationsUseCase(new FakeNotificationRepository());

        var results = useCase.Execute(new GetMyNotificationsQuery(Guid.NewGuid()));

        Assert.Empty(results);
    }
}
