namespace Studia.Application.Notifications;

public record MarkNotificationAsReadCommand(Guid NotificationId, Guid RequestingUserId);
