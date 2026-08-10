using Studia.Application.Users;

namespace Studia.WebApi.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin")
            .RequireAuthorization(policy => policy.RequireRole("Administrador"));

        // A diferencia de /api/auth/register, acá SÍ se acepta el RegisterUserCommand
        // completo (incluye Role) -- porque quien llama ya está autenticado como
        // Administrador. Es el único camino para crear cuentas de Profesor o de otro
        // Administrador; no existe un modo "auto-servicio" para esos roles.
        group.MapPost("/users", (RegisterUserCommand command, IRegisterUserUseCase useCase) =>
        {
            var result = useCase.Execute(command);
            return Results.Created($"/api/users/{result.Id}", result);
        });
    }
}
