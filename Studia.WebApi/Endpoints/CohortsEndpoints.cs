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

        // Versión en lote: el profesor arma la lista buscando con GET /api/users/search y
        // la manda de una sola vez, igual que POST /api/courses/{courseId}/students.
        group.MapPost("/{cohortId:guid}/students", (Guid cohortId, AssignStudentsBody body, IAssignStudentsToCohortUseCase useCase) =>
            Results.Ok(useCase.Execute(new AssignStudentsToCohortCommand(cohortId, body.StudentIdentifiers))));
    }

    private record AssignStudentsBody(IReadOnlyCollection<string> StudentIdentifiers);
}
