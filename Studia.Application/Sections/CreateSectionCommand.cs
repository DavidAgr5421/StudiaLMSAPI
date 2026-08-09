namespace Studia.Application.Sections;

public record CreateSectionCommand(Guid CourseId, string Title, string DescriptionHtml);
