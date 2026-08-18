namespace Studia.Application.Courses;

// Color null limpia la personalización (vuelve al estilo por defecto del front).
public record UpdateCourseColorCommand(Guid CourseId, string? Color);
