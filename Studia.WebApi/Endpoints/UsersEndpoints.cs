using Studia.Application.Users;

namespace Studia.WebApi.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/search", (string q, ISearchUsersUseCase useCase) =>
                Results.Ok(useCase.Execute(new SearchUsersQuery(q))))
            .RequireAuthorization(policy => policy.RequireRole("Administrador", "Profesor"));
    }
}
