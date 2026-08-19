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
        // repetida por cada archivo (tipo File, no Text) y opcionalmente "description" (tipo
        // Text) -- el mismo HTML enriquecido que usa el profesor para describir una sección.
        group.MapPost("/files/{activityId:guid}", async (Guid activityId, HttpContext httpContext, ISubmitFilesActivityUseCase useCase) =>
            {
                var form = await httpContext.Request.ReadFormAsync();
                var description = form.TryGetValue("description", out var descriptionValue) ? descriptionValue.ToString() : null;

                var inputs = new List<SubmittedFileInput>();
                foreach (var file in form.Files)
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    inputs.Add(new SubmittedFileInput(file.FileName, stream.ToArray()));
                }

                var result = useCase.Execute(new SubmitFilesCommand(activityId, httpContext.User.GetUserId(), inputs, description));
                return Results.Created($"/api/submissions/{result.Id}", result);
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        // Edición de una entrega ya hecha -- el dueño sale del JWT, igual que en el resto de
        // los endpoints de "mi propia entrega". El dominio rechaza esto si ya venció el plazo.
        group.MapPut("/{submissionId:guid}/text", (Guid submissionId, EditTextBody body, HttpContext httpContext, IEditTextSubmissionUseCase useCase) =>
                Results.Ok(useCase.Execute(new EditTextSubmissionCommand(submissionId, httpContext.User.GetUserId(), body.TextContent))))
            .RequireAuthorization();

        group.MapPut("/{submissionId:guid}/files", async (Guid submissionId, HttpContext httpContext, IEditFilesSubmissionUseCase useCase) =>
            {
                var form = await httpContext.Request.ReadFormAsync();
                var description = form.TryGetValue("description", out var descriptionValue) ? descriptionValue.ToString() : null;

                var inputs = new List<SubmittedFileInput>();
                foreach (var file in form.Files)
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    inputs.Add(new SubmittedFileInput(file.FileName, stream.ToArray()));
                }

                var result = useCase.Execute(new EditFilesSubmissionCommand(submissionId, httpContext.User.GetUserId(), inputs, description));
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        // Esta la decide el profesor sobre la entrega de OTRA persona -- id explícito en la ruta.
        group.MapPost("/{submissionId:guid}/grade", (Guid submissionId, GradeBody body, IGradeSubmissionUseCase useCase) =>
                Results.Ok(useCase.Execute(new GradeSubmissionCommand(submissionId, body.Score, body.Feedback))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        // Descarga de un archivo entregado -- a diferencia del material de apoyo, esto es
        // trabajo privado del estudiante: solo el dueño, el profesor del curso o un admin.
        group.MapGet("/{submissionId:guid}/files/{storageKey}", (Guid submissionId, string storageKey, HttpContext httpContext, IGetSubmissionFileUseCase useCase) =>
            {
                var isAdmin = httpContext.User.IsInRole("Administrador");
                var result = useCase.Execute(new GetSubmissionFileQuery(submissionId, storageKey, httpContext.User.GetUserId(), isAdmin));
                return Results.File(result.Content, "application/octet-stream", result.FileName);
            })
            .RequireAuthorization();
    }

    private record SubmitTextBody(Guid ActivityId, string TextContent);

    private record EditTextBody(string TextContent);

    private record GradeBody(int Score, string? Feedback);
}
