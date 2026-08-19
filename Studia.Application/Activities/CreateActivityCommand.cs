using Studia.Domain.Activities;

namespace Studia.Application.Activities;

// CohortIds vacío o null = actividad global. Files son los materiales de apoyo que
// sube el profesor (no confundir con lo que entrega el estudiante).
public record CreateActivityCommand(
    Guid SectionId,
    string Title,
    string Description,
    DateTime DueDateUtc,
    ActivityType Type,
    int? MaxFiles,
    IReadOnlyCollection<Guid>? CohortIds = null,
    IReadOnlyCollection<ActivityFileInput>? Files = null,
    ActivityStatus Status = ActivityStatus.Visible);
