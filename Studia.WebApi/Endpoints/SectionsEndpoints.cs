using Studia.Application.Activities;
using Studia.Application.Sections;

namespace Studia.WebApi.Endpoints;

public static class SectionsEndpoints
{
    public static void MapSectionsEndpoints(this WebApplication app)
    {
        // Ojo: acá NO va un RequireAuthorization a nivel de grupo -- crear una sección es
        // solo de Profesor/Administrador, pero listar las actividades de una sección es
        // para cualquier usuario logueado (un estudiante también necesita verlas).
        var group = app.MapGroup("/api/sections");

        group.MapPost("/", (CreateSectionCommand command, ICreateSectionUseCase useCase) =>
            {
                var result = useCase.Execute(command);
                return Results.Created($"/api/sections/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // El rol/id del que pregunta decide qué actividades ve -- un estudiante solo las
        // globales o las de sus fichas, el profesor/admin las ve todas.
        group.MapGet("/{sectionId:guid}/activities", (Guid sectionId, HttpContext httpContext, IGetActivitiesBySectionUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetActivitiesBySectionQuery(sectionId, httpContext.User.GetUserId(), httpContext.User.GetRole()))))
            .RequireAuthorization();

        group.MapDelete("/{sectionId:guid}", (Guid sectionId, IDeleteSectionUseCase useCase) =>
            {
                useCase.Execute(new DeleteSectionCommand(sectionId));
                return Results.NoContent();
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }
}
