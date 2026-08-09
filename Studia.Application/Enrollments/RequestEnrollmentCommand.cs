namespace Studia.Application.Enrollments;

public record RequestEnrollmentCommand(Guid CourseId, Guid StudentId);
