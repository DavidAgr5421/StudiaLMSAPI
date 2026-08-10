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

        group.MapGet("/{sectionId:guid}/activities", (Guid sectionId, IGetActivitiesBySectionUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetActivitiesBySectionQuery(sectionId))))
            .RequireAuthorization();
    }
}
