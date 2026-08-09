using Studia.Domain.Courses;

namespace Studia.Application.Courses;

public record CourseResult(Guid Id, string Name, EnrollmentMode EnrollmentMode, CourseStatus Status, string? InvitationCode)
{
    public static CourseResult FromDomain(Course course) =>
        new(course.Id, course.Name, course.EnrollmentMode, course.Status, course.InvitationCode);
}
