using Studia.Domain.Enrollments;

namespace Studia.Application.Enrollments;

public record EnrollmentResult(Guid Id, Guid CourseId, Guid StudentId, EnrollmentStatus Status, DateTime RequestedAtUtc, DateTime? DecidedAtUtc)
{
    public static EnrollmentResult FromDomain(Enrollment enrollment) =>
        new(enrollment.Id, enrollment.CourseId, enrollment.StudentId, enrollment.Status, enrollment.RequestedAtUtc, enrollment.DecidedAtUtc);
}
