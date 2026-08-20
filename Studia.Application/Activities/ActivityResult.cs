using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public record ActivityResult(
    Guid Id,
    Guid SectionId,
    string Title,
    string Description,
    DateTime DueDateUtc,
    ActivityType Type,
    int? MaxFiles,
    IReadOnlyCollection<Guid> CohortIds,
    IReadOnlyCollection<ActivityFileResult> Files,
    ActivityStatus Status,
    ActivityKind Kind,
    DateTime? OpenDateUtc,
    bool AllowsLateSubmission,
    bool IsManuallyClosed,
    // Refleja Activity.AcceptsSubmissionsAt(ahora) -- el front no tiene que reimplementar
    // la regla de fecha límite/cierre manual, solo mostrar/ocultar en base a esto.
    bool AcceptsSubmissions,
    // Refleja Activity.HasOpenedAt(ahora). El profesor/admin siempre recibe la actividad
    // igual (nunca se les oculta), así que este campo es lo único que le indica al front
    // si todavía no es visible para los estudiantes.
    bool HasOpened,
    // Solo lo completa GetActivityByIdUseCase (resuelve Sección -> Curso) -- los listados
    // por sección no lo necesitan porque el caller ya conoce el curso por contexto.
    Guid? CourseId = null)
{
    public static ActivityResult FromDomain(Activity activity) =>
        new(
            activity.Id,
            activity.SectionId,
            activity.Title,
            activity.Description,
            activity.DueDateUtc,
            activity.Type,
            activity.MaxFiles,
            activity.CohortIds,
            activity.Files.Select(ActivityFileResult.FromDomain).ToList(),
            activity.Status,
            activity.Kind,
            activity.OpenDateUtc,
            activity.AllowsLateSubmission,
            activity.ManuallyClosedAtUtc is not null,
            activity.AcceptsSubmissionsAt(DateTime.UtcNow),
            activity.HasOpenedAt(DateTime.UtcNow));
}
