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
            activity.Files.Select(ActivityFileResult.FromDomain).ToList());
}
