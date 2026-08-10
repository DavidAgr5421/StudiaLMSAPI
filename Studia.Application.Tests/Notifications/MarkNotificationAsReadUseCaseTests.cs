using Studia.Application.Notifications;
using Studia.Domain.Notifications;

namespace Studia.Application.Tests.Notifications;

public class MarkNotificationAsReadUseCaseTests
{
    [Fact]
    public void Execute_WithOwnNotification_MarksAsRead()
    {
        var repository = new FakeNotificationRepository();
        var recipientId = Guid.NewGuid();
        var notification = Notification.Create(recipientId, NotificationType.NuevaActividad, "Título", "Mensaje");
        repository.Save(notification);
        var useCase = new MarkNotificationAsReadUseCase(repository);

        var result = useCase.Execute(new MarkNotificationAsReadCommand(notification.Id, recipientId));

        Assert.NotNull(result.ReadAtUtc);
    }

    [Fact]
    public void Execute_WhenNotificationDoesNotExist_Throws()
    {
        var useCase = new MarkNotificationAsReadUseCase(new FakeNotificationRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new MarkNotificationAsReadCommand(Guid.NewGuid(), Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenNotificationBelongsToAnotherUser_Throws()
    {
        var repository = new FakeNotificationRepository();
        var notification = Notification.Create(Guid.NewGuid(), NotificationType.NuevaActividad, "Título", "Mensaje");
        repository.Save(notification);
        var useCase = new MarkNotificationAsReadUseCase(repository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new MarkNotificationAsReadCommand(notification.Id, Guid.NewGuid())));
    }
}
