using Studia.Application.Courses;
using Studia.Application.Enrollments;
using Studia.Application.Sections;

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

        // También pública: RF12 permite ver el detalle de un curso antes de loguearse.
        group.MapGet("/{courseId:guid}", (Guid courseId, IGetCourseByIdUseCase useCase) =>
        {
            var result = useCase.Execute(new GetCourseByIdQuery(courseId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        // El contenido sí requiere sesión -- a diferencia del curso en sí, las secciones
        // no son parte del preview público.
        group.MapGet("/{courseId:guid}/sections", (Guid courseId, IGetSectionsByCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetSectionsByCourseQuery(courseId))))
            .RequireAuthorization();

        // RF11: el profesor arma la lista (buscando con GET /api/users/search) y la manda de una
        // sola vez. Cada identificador puede ser email o id; si uno falla no tumba a los demás --
        // revisá "outcomes" en la respuesta para ver cuál entró y cuál no.
        group.MapPost("/{courseId:guid}/students", (Guid courseId, AddStudentsBody body, IAddStudentsToCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new AddStudentsToCourseCommand(courseId, body.StudentIdentifiers))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }

    private record AddStudentsBody(IReadOnlyCollection<string> StudentIdentifiers);
}
