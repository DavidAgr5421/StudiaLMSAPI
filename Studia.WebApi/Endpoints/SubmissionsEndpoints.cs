using Studia.Application.Submissions;

namespace Studia.WebApi.Endpoints;

public static class SubmissionsEndpoints
{
    public static void MapSubmissionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/submissions");

        // Auto-servicio (JSON): el estudiante entrega su propio texto.
        group.MapPost("/text", (SubmitTextBody body, HttpContext httpContext, ISubmitTextActivityUseCase useCase) =>
            {
                var result = useCase.Execute(new SubmitTextCommand(body.ActivityId, httpContext.User.GetUserId(), body.TextContent));
                return Results.Created($"/api/submissions/{result.Id}", result);
            })
            .RequireAuthorization();

        // Auto-servicio (multipart/form-data): en Postman, Body -> form-data, una key "files"
        // repetida por cada archivo (tipo File, no Text).
        group.MapPost("/files/{activityId:guid}", async (Guid activityId, IFormFileCollection files, HttpContext httpContext, ISubmitFilesActivityUseCase useCase) =>
            {
                var inputs = new List<SubmittedFileInput>();
                foreach (var file in files)
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    inputs.Add(new SubmittedFileInput(file.FileName, stream.ToArray()));
                }

                var result = useCase.Execute(new SubmitFilesCommand(activityId, httpContext.User.GetUserId(), inputs));
                return Results.Created($"/api/submissions/{result.Id}", result);
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        // Esta la decide el profesor sobre la entrega de OTRA persona -- id explícito en la ruta.
        group.MapPost("/{submissionId:guid}/grade", (Guid submissionId, GradeBody body, IGradeSubmissionUseCase useCase) =>
                Results.Ok(useCase.Execute(new GradeSubmissionCommand(submissionId, body.Score, body.Feedback))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }

    private record SubmitTextBody(Guid ActivityId, string TextContent);

    private record GradeBody(int Score, string? Feedback);
}
