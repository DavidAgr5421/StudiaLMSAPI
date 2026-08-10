namespace Studia.Application.Enrollments;

public record AddStudentToCourseOutcome(string Identifier, bool Success, string? ErrorMessage, EnrollmentResult? Enrollment);
