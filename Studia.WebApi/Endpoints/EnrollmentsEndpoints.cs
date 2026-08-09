using Studia.Application.Enrollments;

namespace Studia.WebApi.Endpoints;

public static class EnrollmentsEndpoints
{
    public static void MapEnrollmentsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/enrollments");

        // Auto-servicio: el estudiante se inscribe a sí mismo, el id sale del JWT.
        group.MapPost("/open", (EnrollOpenBody body, HttpContext httpContext, IEnrollStudentInOpenCourseUseCase useCase) =>
            {
                var result = useCase.Execute(new EnrollStudentInOpenCourseCommand(body.CourseId, httpContext.User.GetUserId()));
                return Results.Created($"/api/enrollments/{result.Id}", result);
            })
            .RequireAuthorization();

        group.MapPost("/requests", (EnrollOpenBody body, HttpContext httpContext, IRequestEnrollmentUseCase useCase) =>
            {
                var result = useCase.Execute(new RequestEnrollmentCommand(body.CourseId, httpContext.User.GetUserId()));
                return Results.Created($"/api/enrollments/{result.Id}", result);
            })
            .RequireAuthorization();

        group.MapPost("/invitation", (EnrollInvitationBody body, HttpContext httpContext, IEnrollByInvitationUseCase useCase) =>
            {
                var result = useCase.Execute(new EnrollByInvitationCommand(body.InvitationCode, httpContext.User.GetUserId()));
                return Results.Created($"/api/enrollments/{result.Id}", result);
            })
            .RequireAuthorization();

        // Estas dos sí las decide el profesor sobre la inscripción de OTRA persona.
        group.MapPost("/{enrollmentId:guid}/approve", (Guid enrollmentId, IApproveEnrollmentUseCase useCase) =>
                Results.Ok(useCase.Execute(new ApproveEnrollmentCommand(enrollmentId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));

        group.MapPost("/{enrollmentId:guid}/reject", (Guid enrollmentId, IRejectEnrollmentUseCase useCase) =>
                Results.Ok(useCase.Execute(new RejectEnrollmentCommand(enrollmentId))))
            .RequireAuthorization(policy => policy.RequireRole("Profesor", "Administrador"));
    }

    private record EnrollOpenBody(Guid CourseId);

    private record EnrollInvitationBody(string InvitationCode);
}
