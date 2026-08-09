using Studia.Application.Sections;

namespace Studia.WebApi.Endpoints;

public static class SectionsEndpoints
{
    public static void MapSectionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sections")
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/", (CreateSectionCommand command, ICreateSectionUseCase useCase) =>
        {
            var result = useCase.Execute(command);
            return Results.Created($"/api/sections/{result.Id}", result);
        });
    }
}
