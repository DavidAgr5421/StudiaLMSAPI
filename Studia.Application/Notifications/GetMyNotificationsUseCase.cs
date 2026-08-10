namespace Studia.Application.Notifications;

public class GetMyNotificationsUseCase(INotificationRepository notificationRepository) : IGetMyNotificationsUseCase
{
    public IReadOnlyCollection<NotificationResult> Execute(GetMyNotificationsQuery query) =>
        notificationRepository.GetByRecipientUserId(query.RecipientUserId)
            .Select(NotificationResult.FromDomain)
            .ToList();
}
