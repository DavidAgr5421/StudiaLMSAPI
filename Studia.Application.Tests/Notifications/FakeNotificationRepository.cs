using Studia.Application.Notifications;
using Studia.Domain.Notifications;

namespace Studia.Application.Tests.Notifications;

public class FakeNotificationRepository : INotificationRepository
{
    private readonly Dictionary<Guid, Notification> _notifications = new();

    public IReadOnlyCollection<Notification> SavedNotifications => _notifications.Values.ToList();

    public void Save(Notification notification) => _notifications[notification.Id] = notification;

    public Notification? GetById(Guid id) => _notifications.GetValueOrDefault(id);

    public IReadOnlyCollection<Notification> GetByRecipientUserId(Guid recipientUserId) =>
        _notifications.Values.Where(n => n.RecipientUserId == recipientUserId).ToList();
}
