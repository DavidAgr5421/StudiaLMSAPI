using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Application.Tests.Enrollments;

public class RejectEnrollmentUseCaseTests
{
    [Fact]
    public void Execute_WhenPending_RejectsEnrollment()
    {
        var repository = new FakeEnrollmentRepository();
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());
        repository.Save(enrollment);
        var useCase = new RejectEnrollmentUseCase(repository);

        var result = useCase.Execute(new RejectEnrollmentCommand(enrollment.Id));

        Assert.Equal(EnrollmentStatus.Rechazada, result.Status);
        Assert.NotNull(result.DecidedAtUtc);
    }

    [Fact]
    public void Execute_WhenEnrollmentDoesNotExist_Throws()
    {
        var useCase = new RejectEnrollmentUseCase(new FakeEnrollmentRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new RejectEnrollmentCommand(Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenAlreadyRejected_Throws()
    {
        var repository = new FakeEnrollmentRepository();
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Reject();
        repository.Save(enrollment);
        var useCase = new RejectEnrollmentUseCase(repository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new RejectEnrollmentCommand(enrollment.Id)));
    }
}
