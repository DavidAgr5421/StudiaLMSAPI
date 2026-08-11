using Studia.Application.Enrollments;
using Studia.Application.Tests.Users;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Enrollments;

public class GetEnrollmentsByCourseUseCaseTests
{
    [Fact]
    public void Execute_IncludesStudentNameAndEmail()
    {
        var enrollments = new FakeEnrollmentRepository();
        var courseId = Guid.NewGuid();
        var student = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana Torres");
        var enrollment = Enrollment.Enroll(courseId, student.Id);
        enrollments.Save(enrollment);

        var users = new FakeUserRepository();
        users.Save(student);

        var useCase = new GetEnrollmentsByCourseUseCase(enrollments, users);

        var result = Assert.Single(useCase.Execute(new GetEnrollmentsByCourseQuery(courseId)));

        Assert.Equal("Ana Torres", result.StudentName);
        Assert.Equal("ana@sena.edu.co", result.StudentEmail);
    }

    [Fact]
    public void Execute_WhenStudentHasNoName_LeavesStudentNameNull()
    {
        var enrollments = new FakeEnrollmentRepository();
        var courseId = Guid.NewGuid();
        var student = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante);
        var enrollment = Enrollment.Enroll(courseId, student.Id);
        enrollments.Save(enrollment);

        var users = new FakeUserRepository();
        users.Save(student);

        var useCase = new GetEnrollmentsByCourseUseCase(enrollments, users);

        var result = Assert.Single(useCase.Execute(new GetEnrollmentsByCourseQuery(courseId)));

        Assert.Null(result.StudentName);
        Assert.Equal("ana@sena.edu.co", result.StudentEmail);
    }

    [Fact]
    public void Execute_ReturnsOnlyEnrollmentsOfThatCourse()
    {
        var enrollments = new FakeEnrollmentRepository();
        var matching = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());
        var other = Enrollment.Enroll(Guid.NewGuid(), Guid.NewGuid());
        enrollments.Save(matching);
        enrollments.Save(other);

        var useCase = new GetEnrollmentsByCourseUseCase(enrollments, new FakeUserRepository());

        var result = Assert.Single(useCase.Execute(new GetEnrollmentsByCourseQuery(matching.CourseId)));

        Assert.Equal(matching.Id, result.Id);
    }
}
