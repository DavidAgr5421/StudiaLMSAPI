using Studia.Application.Courses;

namespace Studia.WebApi.Endpoints;

public static class CoursesEndpoints
{
    public static void MapCoursesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/courses");

        group.MapPost("/", (CreateCourseCommand command, ICreateCourseUseCase useCase) =>
            {
                var result = useCase.Execute(command);
                return Results.Created($"/api/courses/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Pública a propósito (RF12): un visitante sin sesión puede buscar cursos.
        group.MapGet("/search", (string q, ISearchCoursesUseCase useCase) =>
            Results.Ok(useCase.Execute(new SearchCoursesQuery(q))));
    }
}
