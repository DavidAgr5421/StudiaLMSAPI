using Microsoft.EntityFrameworkCore;
using Studia.Application.Notifications;
using Studia.Domain.Notifications;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfNotificationRepository(StudiaDbContext dbContext) : INotificationRepository
{
    public void Save(Notification notification)
    {
        if (dbContext.Notifications.Any(n => n.Id == notification.Id))
            dbContext.Notifications.Update(notification);
        else
            dbContext.Notifications.Add(notification);

        dbContext.SaveChanges();
    }

    public Notification? GetById(Guid id) => dbContext.Notifications.FirstOrDefault(n => n.Id == id);

    public IReadOnlyCollection<Notification> GetByRecipientUserId(Guid recipientUserId) =>
        dbContext.Notifications.Where(n => n.RecipientUserId == recipientUserId).ToList();
}
