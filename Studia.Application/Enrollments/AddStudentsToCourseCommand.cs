namespace Studia.Application.Enrollments;

public record AddStudentsToCourseCommand(Guid CourseId, IReadOnlyCollection<string> StudentIdentifiers);
