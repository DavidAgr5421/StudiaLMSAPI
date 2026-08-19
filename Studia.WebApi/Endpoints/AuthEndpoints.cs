using System.Security.Claims;
using Studia.Application.Auth;
using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        // Público a propósito, y por eso mismo el body NO tiene un campo Role: si lo
        // tuviera, alcanzaría con mandar "role": "Administrador" (o incluso omitir el
        // campo -- un enum ausente en el JSON se rellena con su valor 0, que en Role es
        // justo Administrador) para autoasignarse cualquier rol. Acá no hay ambigüedad
        // posible: todo lo que entra por este endpoint es Estudiante, sin excepción.
        group.MapPost("/register", (PublicRegisterBody body, IRegisterUserUseCase useCase) =>
        {
            var command = new RegisterUserCommand(body.Email, body.Password, Role.Estudiante, body.Name);
            var result = useCase.Execute(command);
            return Results.Created($"/api/users/{result.Id}", result);
        });

        group.MapPost("/login", (LoginCommand command, ILoginUseCase useCase) =>
        {
            try
            {
                return Results.Ok(useCase.Execute(command));
            }
            catch (InvalidOperationException ex)
            {
                // Caso especial: un fallo de credenciales es 401, no el 409 genérico
                // que el manejador global le da al resto de las InvalidOperationException.
                return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status401Unauthorized, title: "No autorizado");
            }
        });

        // Responde 200 con el mismo mensaje sin importar si el email existe o no --
        // si no, cualquiera podría usar este endpoint para enumerar cuentas registradas.
        group.MapPost("/forgot-password", (ForgotPasswordBody body, IRequestPasswordResetUseCase useCase) =>
        {
            useCase.Execute(new RequestPasswordResetCommand(body.Email));
            return Results.Ok(new { message = "Si el email está registrado, vas a recibir instrucciones para restablecer tu contraseña." });
        });

        group.MapPost("/reset-password", (ResetPasswordBody body, IResetPasswordUseCase useCase) =>
        {
            useCase.Execute(new ResetPasswordCommand(body.Token, body.NewPassword));
            return Results.Ok(new { message = "Contraseña actualizada. Ya podés iniciar sesión." });
        });

        group.MapPost("/logout", (HttpContext httpContext, ILogoutUseCase useCase) =>
            {
                var token = ExtractBearerToken(httpContext);
                useCase.Execute(new LogoutCommand(token));
                return Results.NoContent();
            })
            .RequireAuthorization();

        // Bonus: para confirmar rápido en Postman que un token realmente sirve, sin
        // tocar ningún caso de uso -- solo lee los claims que la firma ya validó.
        group.MapGet("/me", (ClaimsPrincipal user) => Results.Ok(new
            {
                UserId = user.GetUserId(),
                Email = user.FindFirst("email")?.Value,
                Role = user.FindFirst(ClaimTypes.Role)?.Value
            }))
            .RequireAuthorization();
    }

    private static string ExtractBearerToken(HttpContext httpContext)
    {
        var header = httpContext.Request.Headers.Authorization.ToString();
        const string prefix = "Bearer ";
        return header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? header[prefix.Length..] : header;
    }

    private record PublicRegisterBody(string Email, string Password, string? Name);

    private record ForgotPasswordBody(string Email);

    private record ResetPasswordBody(string Token, string NewPassword);
}
