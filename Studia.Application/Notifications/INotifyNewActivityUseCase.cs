namespace Studia.Application.Notifications;

public interface INotifyNewActivityUseCase
{
    IReadOnlyCollection<NotificationResult> Execute(NotifyNewActivityCommand command);
}
