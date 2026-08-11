using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Application.Tests.Enrollments;

public class GetEnrollmentsByStudentUseCaseTests
{
    [Fact]
    public void Execute_ReturnsOnlyEnrollmentsOfThatStudent()
    {
        var enrollments = new FakeEnrollmentRepository();
        var studentId = Guid.NewGuid();
        var mine = Enrollment.Enroll(Guid.NewGuid(), studentId);
        var other = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());
        enrollments.Save(mine);
        enrollments.Save(other);

        var useCase = new GetEnrollmentsByStudentUseCase(enrollments);

        var result = Assert.Single(useCase.Execute(new GetEnrollmentsByStudentQuery(studentId)));

        Assert.Equal(mine.Id, result.Id);
    }

    [Fact]
    public void Execute_WhenStudentHasNoEnrollments_ReturnsEmpty()
    {
        var useCase = new GetEnrollmentsByStudentUseCase(new FakeEnrollmentRepository());

        var result = useCase.Execute(new GetEnrollmentsByStudentQuery(Guid.NewGuid()));

        Assert.Empty(result);
    }
}
