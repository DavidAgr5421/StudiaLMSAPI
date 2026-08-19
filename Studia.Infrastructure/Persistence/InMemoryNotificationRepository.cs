using System.Collections.Concurrent;
using Studia.Application.Notifications;
using Studia.Domain.Notifications;

namespace Studia.Infrastructure.Persistence;

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly ConcurrentDictionary<Guid, Notification> _notifications = new();

    public void Save(Notification notification) => _notifications[notification.Id] = notification;

    public Notification? GetById(Guid id) => _notifications.GetValueOrDefault(id);

    public IReadOnlyCollection<Notification> GetByRecipientUserId(Guid recipientUserId) =>
        _notifications.Values.Where(n => n.RecipientUserId == recipientUserId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToList();
}
