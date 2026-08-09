using Studia.Application.Activities;

namespace Studia.WebApi.Endpoints;

public static class ActivitiesEndpoints
{
    public static void MapActivitiesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/activities")
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/", (CreateActivityCommand command, ICreateActivityUseCase useCase) =>
        {
            var result = useCase.Execute(command);
            return Results.Created($"/api/activities/{result.Id}", result);
        });
    }
}
