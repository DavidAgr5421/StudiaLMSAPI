namespace Studia.Application.Notifications;

public interface ISendDueDateReminderUseCase
{
    IReadOnlyCollection<NotificationResult> Execute(SendDueDateReminderCommand command);
}
