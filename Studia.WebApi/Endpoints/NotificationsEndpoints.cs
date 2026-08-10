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

        // El destinatario sale del JWT, no del body: así nadie puede marcar como leída
        // una notificación ajena solo por adivinar su id.
        group.MapPost("/{notificationId:guid}/read", (Guid notificationId, HttpContext httpContext, IMarkNotificationAsReadUseCase useCase) =>
                Results.Ok(useCase.Execute(new MarkNotificationAsReadCommand(notificationId, httpContext.User.GetUserId()))))
            .RequireAuthorization();

        // Mismo criterio: la lista es siempre "las mías", nunca las de un id arbitrario.
        group.MapGet("/me", (HttpContext httpContext, IGetMyNotificationsUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetMyNotificationsQuery(httpContext.User.GetUserId()))))
            .RequireAuthorization();
    }
}
