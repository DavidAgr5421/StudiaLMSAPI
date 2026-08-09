namespace Studia.Application.Enrollments;

public record EnrollStudentInOpenCourseCommand(Guid CourseId, Guid StudentId);
