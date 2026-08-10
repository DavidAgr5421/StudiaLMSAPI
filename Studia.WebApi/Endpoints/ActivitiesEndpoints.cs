using Studia.Application.Activities;
using Studia.Application.Submissions;

namespace Studia.WebApi.Endpoints;

public static class ActivitiesEndpoints
{
    public static void MapActivitiesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/activities");

        group.MapPost("/", (CreateActivityCommand command, ICreateActivityUseCase useCase) =>
            {
                var result = useCase.Execute(command);
                return Results.Created($"/api/activities/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Vista de calificación del profesor -- un estudiante no tiene por qué ver las
        // entregas de sus compañeros, así que esta sí queda restringida por rol.
        group.MapGet("/{activityId:guid}/submissions", (Guid activityId, IGetSubmissionsByActivityUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetSubmissionsByActivityQuery(activityId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }
}
