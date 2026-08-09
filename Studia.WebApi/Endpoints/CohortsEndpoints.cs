using Studia.Application.Cohorts;

namespace Studia.WebApi.Endpoints;

public static class CohortsEndpoints
{
    public static void MapCohortsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/cohorts")
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/", (CreateCohortCommand command, ICreateCohortUseCase useCase) =>
        {
            var result = useCase.Execute(command);
            return Results.Created($"/api/cohorts/{result.Id}", result);
        });

        // Acá el profesor sí indica explícitamente qué estudiante asigna -- no es
        // auto-servicio, así que el id viaja en el body, no se toma del JWT.
        group.MapPost("/students", (AssignStudentToCohortCommand command, IAssignStudentToCohortUseCase useCase) =>
            Results.Ok(useCase.Execute(command)));
    }
}
