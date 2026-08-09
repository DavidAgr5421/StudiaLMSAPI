using System.Security.Claims;
using Studia.Application.Auth;
using Studia.Application.Users;

namespace Studia.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", (RegisterUserCommand command, IRegisterUserUseCase useCase) =>
        {
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
}
