using Studia.Domain.Enrollments;

namespace Studia.Domain.Tests.Enrollments;

public class EnrollmentTests
{
    [Fact]
    public void Enroll_WithValidIds_StartsApproved()
    {
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var enrollment = Enrollment.Enroll(courseId, studentId);

        Assert.Equal(courseId, enrollment.CourseId);
        Assert.Equal(studentId, enrollment.StudentId);
        Assert.Equal(EnrollmentStatus.Aprobada, enrollment.Status);
        Assert.InRange(enrollment.RequestedAtUtc, before, DateTime.UtcNow);
        Assert.NotNull(enrollment.DecidedAtUtc);
    }

    [Fact]
    public void Enroll_WithEmptyCourseId_Throws()
    {
        Assert.Throws<ArgumentException>(() => Enrollment.Enroll(Guid.Empty, Guid.NewGuid()));
    }

    [Fact]
    public void Enroll_WithEmptyStudentId_Throws()
    {
        Assert.Throws<ArgumentException>(() => Enrollment.Enroll(Guid.NewGuid(), Guid.Empty));
    }

    [Fact]
    public void RequestEnrollment_WithValidIds_StartsPendingWithoutDecision()
    {
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(EnrollmentStatus.Pendiente, enrollment.Status);
        Assert.Null(enrollment.DecidedAtUtc);
    }

    [Fact]
    public void Approve_WhenPending_SetsStatusAprobadaAndDecidedAt()
    {
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());

        enrollment.Approve();

        Assert.Equal(EnrollmentStatus.Aprobada, enrollment.Status);
        Assert.NotNull(enrollment.DecidedAtUtc);
    }

    [Fact]
    public void Approve_WhenNotPending_Throws()
    {
        var enrollment = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => enrollment.Approve());
    }

    [Fact]
    public void Reject_WhenPending_SetsStatusRechazadaAndDecidedAt()
    {
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());

        enrollment.Reject();

        Assert.Equal(EnrollmentStatus.Rechazada, enrollment.Status);
        Assert.NotNull(enrollment.DecidedAtUtc);
    }

    [Fact]
    public void Reject_WhenNotPending_Throws()
    {
        var enrollment = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => enrollment.Reject());
    }
}
