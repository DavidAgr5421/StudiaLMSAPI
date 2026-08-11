using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.WebApi.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/search", (string q, ISearchUsersUseCase useCase) =>
                Results.Ok(useCase.Execute(new SearchUsersQuery(q))))
            .RequireAuthorization(policy => policy.RequireRole("Administrador", "Profesor"));

        // Para el modal de "info del estudiante" en la vista de entregas -- el profesor
        // necesita ver el perfil de OTRO usuario, así que el id sí viaja en la ruta (a
        // diferencia de todo lo que cuelga de /me). Restringido por el mismo motivo que
        // /search: no cualquiera debería poder consultar el documento de identidad ajeno.
        app.MapGet("/api/users/{userId:guid}", (Guid userId, IGetUserByIdUseCase useCase) =>
            {
                var result = useCase.Execute(new GetUserByIdQuery(userId));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Administrador", "Profesor"));

        var meGroup = app.MapGroup("/api/users/me").RequireAuthorization();

        // "Mi perfil": todo auto-servicio, el id sale siempre del JWT -- nadie puede leer o
        // tocar el perfil de otro usuario por esta vía, sin importar el rol.
        meGroup.MapGet("/", (HttpContext httpContext, IGetUserByIdUseCase useCase) =>
        {
            var result = useCase.Execute(new GetUserByIdQuery(httpContext.User.GetUserId()));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        meGroup.MapPatch("/name", (UpdateNameBody body, HttpContext httpContext, IUpdateNameUseCase useCase) =>
            Results.Ok(useCase.Execute(new UpdateNameCommand(httpContext.User.GetUserId(), body.Name))));

        meGroup.MapPost("/email", (ChangeEmailBody body, HttpContext httpContext, IChangeEmailUseCase useCase) =>
            Results.Ok(useCase.Execute(new ChangeEmailCommand(httpContext.User.GetUserId(), body.NewEmail, body.CurrentPassword))));

        meGroup.MapPost("/password", (ChangePasswordBody body, HttpContext httpContext, IChangePasswordUseCase useCase) =>
        {
            useCase.Execute(new ChangePasswordCommand(httpContext.User.GetUserId(), body.CurrentPassword, body.NewPassword));
            return Results.NoContent();
        });

        meGroup.MapPatch("/identification", (SetIdentificationBody body, HttpContext httpContext, ISetIdentificationUseCase useCase) =>
            Results.Ok(useCase.Execute(new SetIdentificationCommand(httpContext.User.GetUserId(), body.TypeId, body.ValueId))));
    }

    private record UpdateNameBody(string? Name);

    private record ChangeEmailBody(string NewEmail, string CurrentPassword);

    private record ChangePasswordBody(string CurrentPassword, string NewPassword);

    private record SetIdentificationBody(IdentificationType TypeId, string ValueId);
}
