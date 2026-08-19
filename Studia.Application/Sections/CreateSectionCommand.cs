using Studia.Domain.Sections;

namespace Studia.Application.Sections;

// CohortIds vacío o null = sección global (visible para todo el curso). Si se indican
// fichas, solo sus estudiantes pueden verla. Status Oculto = solo la ve el profesor
// dueño del curso (o un admin), no dispara notificaciones.
public record CreateSectionCommand(
    Guid CourseId,
    string Title,
    string DescriptionHtml,
    IReadOnlyCollection<Guid>? CohortIds = null,
    SectionStatus Status = SectionStatus.Visible);
