using Studia.Application.Enrollments;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Enrollments;

public class EnrollByInvitationUseCaseTests
{
    private static (FakeCourseRepository Courses, FakeUserRepository Users, FakeEnrollmentRepository Enrollments, EnrollByInvitationUseCase UseCase) CreateSut()
    {
        var courses = new FakeCourseRepository();
        var users = new FakeUserRepository();
        var enrollments = new FakeEnrollmentRepository();
        var useCase = new EnrollByInvitationUseCase(enrollments, courses, users);

        return (courses, users, enrollments, useCase);
    }

    private static User CreateStudent(string email = "estudiante@sena.edu.co") =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante);

    [Fact]
    public void Execute_WithValidCode_EnrollsStudentAsApproved()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English C1", EnrollmentMode.PorInvitacion, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        var result = useCase.Execute(new EnrollByInvitationCommand(course.InvitationCode!, student.Id));

        Assert.Equal(EnrollmentStatus.Aprobada, result.Status);
        Assert.Single(enrollments.SavedEnrollments);
    }

    [Fact]
    public void Execute_WithUnknownCode_Throws()
    {
        var (_, users, _, useCase) = CreateSut();
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollByInvitationCommand("BADCODE1", student.Id)));
    }

    [Fact]
    public void Execute_WhenCourseIsArchived_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English C1", EnrollmentMode.PorInvitacion, Guid.NewGuid());
        course.Archive();
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollByInvitationCommand(course.InvitationCode!, student.Id)));
    }

    [Fact]
    public void Execute_WhenStudentAlreadyEnrolled_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English C1", EnrollmentMode.PorInvitacion, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);
        useCase.Execute(new EnrollByInvitationCommand(course.InvitationCode!, student.Id));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollByInvitationCommand(course.InvitationCode!, student.Id)));
    }
}
