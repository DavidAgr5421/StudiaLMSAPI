using System.Globalization;
using Studia.Application.Activities;
using Studia.Application.Submissions;
using Studia.Domain.Activities;

namespace Studia.WebApi.Endpoints;

public static class ActivitiesEndpoints
{
    public static void MapActivitiesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/activities");

        // multipart/form-data (no JSON): además de los campos de la actividad, el profesor
        // puede adjuntar material de apoyo -- en Postman, Body -> form-data, con "files"
        // repetido por cada archivo (tipo File) y "cohortIds" repetido por cada ficha.
        group.MapPost("/", async (HttpContext httpContext, ICreateActivityUseCase useCase) =>
            {
                var form = await httpContext.Request.ReadFormAsync();

                var sectionId = Guid.Parse(form["sectionId"].ToString());
                var title = form["title"].ToString();
                var description = form["description"].ToString();
                var dueDateUtc = DateTime.Parse(
                    form["dueDateUtc"].ToString(),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                var type = Enum.Parse<ActivityType>(form["type"].ToString(), ignoreCase: true);
                var maxFiles = int.TryParse(form["maxFiles"], out var parsedMaxFiles) ? parsedMaxFiles : (int?)null;
                var cohortIds = form["cohortIds"]
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => Guid.Parse(value!))
                    .ToList();
                var status = Enum.TryParse<ActivityStatus>(form["status"].ToString(), ignoreCase: true, out var parsedStatus)
                    ? parsedStatus
                    : ActivityStatus.Visible;

                var fileInputs = new List<ActivityFileInput>();
                foreach (var file in form.Files)
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    fileInputs.Add(new ActivityFileInput(file.FileName, stream.ToArray()));
                }

                var command = new CreateActivityCommand(sectionId, title, description, dueDateUtc, type, maxFiles, cohortIds, fileInputs, status);
                var result = useCase.Execute(command);
                return Results.Created($"/api/activities/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"))
            .DisableAntiforgery();

        // Detalle de una actividad por id -- la página de actividad (antes modal) la carga
        // directo desde la URL, sin pasar por el listado de la sección. Igual que la
        // descarga de material: cualquier usuario logueado, sin filtrar por ficha acá
        // (ese filtrado ya pasó al listar la sección/actividad).
        group.MapGet("/{activityId:guid}", (Guid activityId, HttpContext httpContext, IGetActivityByIdUseCase useCase) =>
            {
                var result = useCase.Execute(new GetActivityByIdQuery(activityId, httpContext.User.GetUserId(), httpContext.User.GetRole()));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .RequireAuthorization();

        // Vista de calificación del profesor -- un estudiante no tiene por qué ver las
        // entregas de sus compañeros, así que esta sí queda restringida por rol.
        group.MapGet("/{activityId:guid}/submissions", (Guid activityId, IGetSubmissionsByActivityUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetSubmissionsByActivityQuery(activityId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Descarga del material de apoyo -- cualquier usuario logueado (el filtrado por
        // ficha ya pasó al listar la actividad; acá alcanza con no ser anónimo).
        group.MapGet("/{activityId:guid}/files/{storageKey}", (Guid activityId, string storageKey, IGetActivityFileUseCase useCase) =>
            {
                var result = useCase.Execute(new GetActivityFileQuery(activityId, storageKey));
                return Results.File(result.Content, "application/octet-stream", result.FileName);
            })
            .RequireAuthorization();

        // "Mi entrega": auto-servicio, igual que la descarga de arriba -- el estudiante
        // solo puede ver la suya (el id sale del JWT, no de la query). Sirve para que el
        // front sepa si ya entregó y muestre el estado/nota en vez del formulario.
        group.MapGet("/{activityId:guid}/my-submission", (Guid activityId, HttpContext httpContext, IGetSubmissionForActivityUseCase useCase) =>
            {
                var result = useCase.Execute(new GetSubmissionForActivityQuery(activityId, httpContext.User.GetUserId()));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .RequireAuthorization();
    }
}
