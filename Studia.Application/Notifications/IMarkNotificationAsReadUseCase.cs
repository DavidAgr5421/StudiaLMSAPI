namespace Studia.Application.Notifications;

public interface IMarkNotificationAsReadUseCase
{
    NotificationResult Execute(MarkNotificationAsReadCommand command);
}
