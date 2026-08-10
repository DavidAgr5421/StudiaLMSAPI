namespace Studia.Application.Notifications;

public interface IGetMyNotificationsUseCase
{
    IReadOnlyCollection<NotificationResult> Execute(GetMyNotificationsQuery query);
}
