using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Enrollments;
using Studia.Application.Sections;
using Studia.Domain.Courses;

namespace Studia.WebApi.Endpoints;

public static class CoursesEndpoints
{
    public static void MapCoursesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/courses");

        // El dueño del curso sale del JWT, no del body: auto-servicio, igual que el resto
        // de los "crear algo propio" de la API.
        group.MapPost("/", (CreateCourseBody body, HttpContext httpContext, ICreateCourseUseCase useCase) =>
            {
                var command = new CreateCourseCommand(body.Name, body.EnrollmentMode, httpContext.User.GetUserId());
                var result = useCase.Execute(command);
                return Results.Created($"/api/courses/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Pública a propósito (RF12): un visitante sin sesión puede buscar cursos. "q" es
        // opcional -- sin término, el botón "Cursos" del front lista todo lo disponible.
        group.MapGet("/search", (string? q, ISearchCoursesUseCase useCase) =>
            Results.Ok(useCase.Execute(new SearchCoursesQuery(q ?? ""))));

        // "Mis cursos": los que creó el profesor/admin autenticado.
        group.MapGet("/mine", (HttpContext httpContext, IGetCoursesByProfesorUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetCoursesByProfesorQuery(httpContext.User.GetUserId()))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // También pública: RF12 permite ver el detalle de un curso antes de loguearse.
        group.MapGet("/{courseId:guid}", (Guid courseId, IGetCourseByIdUseCase useCase) =>
        {
            var result = useCase.Execute(new GetCourseByIdQuery(courseId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        // El contenido sí requiere sesión -- a diferencia del curso en sí, las secciones
        // no son parte del preview público. El rol/id del que pregunta decide qué
        // secciones ve (global o de sus fichas), igual que con las actividades.
        group.MapGet("/{courseId:guid}/sections", (Guid courseId, HttpContext httpContext, IGetSectionsByCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetSectionsByCourseQuery(courseId, httpContext.User.GetUserId(), httpContext.User.GetRole()))))
            .RequireAuthorization();

        // RF11: el profesor arma la lista (buscando con GET /api/users/search) y la manda de una
        // sola vez. Cada identificador puede ser email o id; si uno falla no tumba a los demás --
        // revisá "outcomes" en la respuesta para ver cuál entró y cuál no.
        group.MapPost("/{courseId:guid}/students", (Guid courseId, AddStudentsBody body, IAddStudentsToCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new AddStudentsToCourseCommand(courseId, body.StudentIdentifiers))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Para que el profesor vea a quién aprobar/rechazar -- sin esto no hay forma de
        // conocer los enrollmentId pendientes de un curso.
        group.MapGet("/{courseId:guid}/enrollments", (Guid courseId, IGetEnrollmentsByCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetEnrollmentsByCourseQuery(courseId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Para que el profesor pueda elegir fichas al crear una sección/actividad con
        // alcance restringido -- sin esto no hay forma de listar las fichas del curso.
        group.MapGet("/{courseId:guid}/cohorts", (Guid courseId, IGetCohortsByCourseUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetCohortsByCourseQuery(courseId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Borrado real, no archivado: borra en cascada secciones, actividades, entregas,
        // inscripciones y fichas del curso. El front pide confirmación antes de llamarlo.
        group.MapDelete("/{courseId:guid}", (Guid courseId, IDeleteCourseUseCase useCase) =>
            {
                useCase.Execute(new DeleteCourseCommand(courseId));
                return Results.NoContent();
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Personalización visual del curso: color en hex (o null para volver al default).
        group.MapPatch("/{courseId:guid}/color", (Guid courseId, UpdateColorBody body, IUpdateCourseColorUseCase useCase) =>
                Results.Ok(useCase.Execute(new UpdateCourseColorCommand(courseId, body.Color))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // multipart/form-data con una sola key "file" (tipo File) -- reemplaza la imagen
        // anterior si ya había una.
        group.MapPost("/{courseId:guid}/cover-image", async (Guid courseId, HttpContext httpContext, ISetCourseCoverImageUseCase useCase) =>
            {
                var form = await httpContext.Request.ReadFormAsync();
                var file = form.Files.GetFile("file");
                if (file is null)
                    return Results.BadRequest(new { message = "Falta el archivo de imagen ('file')." });

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                var result = useCase.Execute(new SetCourseCoverImageCommand(courseId, file.FileName, stream.ToArray()));
                return Results.Ok(result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"))
            .DisableAntiforgery();

        group.MapDelete("/{courseId:guid}/cover-image", (Guid courseId, IRemoveCourseCoverImageUseCase useCase) =>
                Results.Ok(useCase.Execute(new RemoveCourseCoverImageCommand(courseId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Pública a propósito, igual que el curso: la portada es parte del preview (RF12).
        group.MapGet("/{courseId:guid}/cover-image", (Guid courseId, IGetCourseCoverImageUseCase useCase) =>
        {
            var result = useCase.Execute(new GetCourseCoverImageQuery(courseId));
            return Results.File(result.Content, CoverImageContentType(result.FileName));
        });
    }

    private static string CoverImageContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };

    private record AddStudentsBody(IReadOnlyCollection<string> StudentIdentifiers);

    private record CreateCourseBody(string Name, EnrollmentMode EnrollmentMode);

    private record UpdateColorBody(string? Color);
}
