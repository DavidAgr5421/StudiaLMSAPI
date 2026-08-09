using Studia.Domain.Courses;

namespace Studia.Application.Courses;

public record CreateCourseCommand(string Name, EnrollmentMode EnrollmentMode);
