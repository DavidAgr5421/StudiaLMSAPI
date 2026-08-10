using Studia.Application.Enrollments;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Enrollments;

public class RequestEnrollmentUseCaseTests
{
    private static (FakeCourseRepository Courses, FakeUserRepository Users, FakeEnrollmentRepository Enrollments, RequestEnrollmentUseCase UseCase) CreateSut()
    {
        var courses = new FakeCourseRepository();
        var users = new FakeUserRepository();
        var enrollments = new FakeEnrollmentRepository();
        var useCase = new RequestEnrollmentUseCase(enrollments, courses, users);

        return (courses, users, enrollments, useCase);
    }

    private static User CreateStudent(string email = "estudiante@sena.edu.co") =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante);

    [Fact]
    public void Execute_WithApprovalCourseAndStudent_CreatesPendingEnrollment()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English B1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        var result = useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id));

        Assert.Equal(EnrollmentStatus.Pendiente, result.Status);
        Assert.Null(result.DecidedAtUtc);
        Assert.Single(enrollments.SavedEnrollments);
    }

    [Fact]
    public void Execute_WhenCourseIsNotApprovalMode_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id)));
    }

    [Fact]
    public void Execute_WhenStudentHasPendingRequest_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English B1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);
        useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id)));
    }

    [Fact]
    public void Execute_WhenPreviousRequestWasRejected_AllowsNewRequest()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English B1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);
        var firstRequest = useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id));
        var rejectedEnrollment = enrollments.SavedEnrollments.Single(e => e.Id == firstRequest.Id);
        rejectedEnrollment.Reject();
        enrollments.Save(rejectedEnrollment);

        var result = useCase.Execute(new RequestEnrollmentCommand(course.Id, student.Id));

        Assert.Equal(EnrollmentStatus.Pendiente, result.Status);
    }
}
