namespace Studia.Application.Notifications;

public interface INotifyNewSectionUseCase
{
    IReadOnlyCollection<NotificationResult> Execute(NotifyNewSectionCommand command);
}
