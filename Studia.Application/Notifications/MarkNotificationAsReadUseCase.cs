namespace Studia.Application.Notifications;

public class MarkNotificationAsReadUseCase(INotificationRepository notificationRepository) : IMarkNotificationAsReadUseCase
{
    public NotificationResult Execute(MarkNotificationAsReadCommand command)
    {
        var notification = notificationRepository.GetById(command.NotificationId)
            ?? throw new InvalidOperationException($"No existe una notificación con id '{command.NotificationId}'.");

        notification.MarkAsRead();

        notificationRepository.Save(notification);

        return NotificationResult.FromDomain(notification);
    }
}
