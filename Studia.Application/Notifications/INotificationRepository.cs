using Studia.Domain.Notifications;

namespace Studia.Application.Notifications;

public interface INotificationRepository
{
    void Save(Notification notification);

    Notification? GetById(Guid id);

    IReadOnlyCollection<Notification> GetByRecipientUserId(Guid recipientUserId);
}
