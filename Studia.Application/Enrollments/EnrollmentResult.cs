using Studia.Domain.Enrollments;

namespace Studia.Application.Enrollments;

// StudentName/StudentEmail son opcionales porque FromDomain(Enrollment) no tiene acceso al
// User -- son harina de otro aggregate. Quien sí lo tenga (GetEnrollmentsByCourseUseCase)
// los completa después con una expresión "with".
public record EnrollmentResult(
    Guid Id,
    Guid CourseId,
    Guid StudentId,
    EnrollmentStatus Status,
    DateTime RequestedAtUtc,
    DateTime? DecidedAtUtc,
    string? StudentName = null,
    string? StudentEmail = null)
{
    public static EnrollmentResult FromDomain(Enrollment enrollment) =>
        new(enrollment.Id, enrollment.CourseId, enrollment.StudentId, enrollment.Status, enrollment.RequestedAtUtc, enrollment.DecidedAtUtc);
}
