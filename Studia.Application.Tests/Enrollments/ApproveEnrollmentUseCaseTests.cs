using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Application.Tests.Enrollments;

public class ApproveEnrollmentUseCaseTests
{
    [Fact]
    public void Execute_WhenPending_ApprovesEnrollment()
    {
        var repository = new FakeEnrollmentRepository();
        var enrollment = Enrollment.RequestEnrollment(Guid.NewGuid(), Guid.NewGuid());
        repository.Save(enrollment);
        var useCase = new ApproveEnrollmentUseCase(repository);

        var result = useCase.Execute(new ApproveEnrollmentCommand(enrollment.Id));

        Assert.Equal(EnrollmentStatus.Aprobada, result.Status);
        Assert.NotNull(result.DecidedAtUtc);
    }

    [Fact]
    public void Execute_WhenEnrollmentDoesNotExist_Throws()
    {
        var useCase = new ApproveEnrollmentUseCase(new FakeEnrollmentRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new ApproveEnrollmentCommand(Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenAlreadyApproved_Throws()
    {
        var repository = new FakeEnrollmentRepository();
        var enrollment = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());
        repository.Save(enrollment);
        var useCase = new ApproveEnrollmentUseCase(repository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new ApproveEnrollmentCommand(enrollment.Id)));
    }
}
