namespace Studia.Domain.Notifications;

public class Notification
{
    public Guid Id { get; }
    public Guid RecipientUserId { get; }
    public NotificationType Type { get; }
    public string Title { get; }
    public string Message { get; }
    public Guid? RelatedEntityId { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? ReadAtUtc { get; private set; }
    public bool EmailSent { get; private set; }

    private Notification(
        Guid id,
        Guid recipientUserId,
        NotificationType type,
        string title,
        string message,
        Guid? relatedEntityId,
        DateTime createdAtUtc)
    {
        Id = id;
        RecipientUserId = recipientUserId;
        Type = type;
        Title = title;
        Message = message;
        RelatedEntityId = relatedEntityId;
        CreatedAtUtc = createdAtUtc;
    }

    public static Notification Create(Guid recipientUserId, NotificationType type, string title, string message, Guid? relatedEntityId = null)
    {
        if (recipientUserId == Guid.Empty)
            throw new ArgumentException("El destinatario no es válido.", nameof(recipientUserId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la notificación no puede estar vacío.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("El mensaje de la notificación no puede estar vacío.", nameof(message));

        return new Notification(Guid.NewGuid(), recipientUserId, type, title.Trim(), message.Trim(), relatedEntityId, DateTime.UtcNow);
    }

    public void MarkAsRead() => ReadAtUtc ??= DateTime.UtcNow;

    public void MarkEmailSent() => EmailSent = true;
}
