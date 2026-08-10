namespace Studia.Application.Sections;

// CohortIds vacío o null = sección global (visible para todo el curso). Si se indican
// fichas, solo sus estudiantes pueden verla.
public record CreateSectionCommand(Guid CourseId, string Title, string DescriptionHtml, IReadOnlyCollection<Guid>? CohortIds = null);
