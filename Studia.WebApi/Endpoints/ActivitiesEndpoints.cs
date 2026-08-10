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

                var fileInputs = new List<ActivityFileInput>();
                foreach (var file in form.Files)
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    fileInputs.Add(new ActivityFileInput(file.FileName, stream.ToArray()));
                }

                var command = new CreateActivityCommand(sectionId, title, description, dueDateUtc, type, maxFiles, cohortIds, fileInputs);
                var result = useCase.Execute(command);
                return Results.Created($"/api/activities/{result.Id}", result);
            })
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"))
            .DisableAntiforgery();

        // Vista de calificación del profesor -- un estudiante no tiene por qué ver las
        // entregas de sus compañeros, así que esta sí queda restringida por rol.
        group.MapGet("/{activityId:guid}/submissions", (Guid activityId, IGetSubmissionsByActivityUseCase useCase) =>
                Results.Ok(useCase.Execute(new GetSubmissionsByActivityQuery(activityId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }
}
