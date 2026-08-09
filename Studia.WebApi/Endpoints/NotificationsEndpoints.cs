using Studia.Application.Notifications;

namespace Studia.WebApi.Endpoints;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications");

        group.MapPost("/new-activity/{activityId:guid}", (Guid activityId, INotifyNewActivityUseCase useCase) =>
                Results.Ok(useCase.Execute(new NotifyNewActivityCommand(activityId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/new-section/{sectionId:guid}", (Guid sectionId, INotifyNewSectionUseCase useCase) =>
                Results.Ok(useCase.Execute(new NotifyNewSectionCommand(sectionId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/due-date-reminder/{activityId:guid}", (Guid activityId, ISendDueDateReminderUseCase useCase) =>
                Results.Ok(useCase.Execute(new SendDueDateReminderCommand(activityId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Cualquier usuario autenticado puede marcar como leída -- ver nota sobre esto en la respuesta.
        group.MapPost("/{notificationId:guid}/read", (Guid notificationId, IMarkNotificationAsReadUseCase useCase) =>
                Results.Ok(useCase.Execute(new MarkNotificationAsReadCommand(notificationId))))
            .RequireAuthorization();
    }
}
