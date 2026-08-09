using Studia.Domain.Notifications;

namespace Studia.Application.Notifications;

public record NotificationResult(
    Guid Id,
    Guid RecipientUserId,
    NotificationType Type,
    string Title,
    string Message,
    Guid? RelatedEntityId,
    DateTime CreatedAtUtc,
    DateTime? ReadAtUtc,
    bool EmailSent)
{
    public static NotificationResult FromDomain(Notification notification) =>
        new(
            notification.Id,
            notification.RecipientUserId,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.RelatedEntityId,
            notification.CreatedAtUtc,
            notification.ReadAtUtc,
            notification.EmailSent);
}
