namespace Studia.Domain.Enrollments;

public class Enrollment
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public Guid StudentId { get; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime RequestedAtUtc { get; }
    public DateTime? DecidedAtUtc { get; private set; }

    private Enrollment(Guid id, Guid courseId, Guid studentId, EnrollmentStatus status, DateTime requestedAtUtc, DateTime? decidedAtUtc)
    {
        Id = id;
        CourseId = courseId;
        StudentId = studentId;
        Status = status;
        RequestedAtUtc = requestedAtUtc;
        DecidedAtUtc = decidedAtUtc;
    }

    public static Enrollment Enroll(Guid courseId, Guid studentId)
    {
        ValidateIds(courseId, studentId);

        var now = DateTime.UtcNow;
        return new Enrollment(Guid.NewGuid(), courseId, studentId, EnrollmentStatus.Aprobada, now, now);
    }

    public static Enrollment RequestEnrollment(Guid courseId, Guid studentId)
    {
        ValidateIds(courseId, studentId);

        return new Enrollment(Guid.NewGuid(), courseId, studentId, EnrollmentStatus.Pendiente, DateTime.UtcNow, null);
    }

    public void Approve()
    {
        if (Status != EnrollmentStatus.Pendiente)
            throw new InvalidOperationException("Solo se puede aprobar una inscripción pendiente.");

        Status = EnrollmentStatus.Aprobada;
        DecidedAtUtc = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != EnrollmentStatus.Pendiente)
            throw new InvalidOperationException("Solo se puede rechazar una inscripción pendiente.");

        Status = EnrollmentStatus.Rechazada;
        DecidedAtUtc = DateTime.UtcNow;
    }

    private static void ValidateIds(Guid courseId, Guid studentId)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("El curso no es válido.", nameof(courseId));

        if (studentId == Guid.Empty)
            throw new ArgumentException("El estudiante no es válido.", nameof(studentId));
    }
}
