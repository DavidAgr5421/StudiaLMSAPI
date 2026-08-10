using Studia.Application.Enrollments;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Enrollments;

public class AddStudentsToCourseUseCaseTests
{
    private static (FakeCourseRepository Courses, FakeUserRepository Users, FakeEnrollmentRepository Enrollments, AddStudentsToCourseUseCase UseCase) CreateSut()
    {
        var courses = new FakeCourseRepository();
        var users = new FakeUserRepository();
        var enrollments = new FakeEnrollmentRepository();
        var useCase = new AddStudentsToCourseUseCase(courses, users, enrollments);

        return (courses, users, enrollments, useCase);
    }

    [Fact]
    public void Execute_WithMixOfEmailAndId_EnrollsBothRegardlessOfEnrollmentMode()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var byEmail = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        var byId = User.Register(Email.Create("luis@sena.edu.co"), "hash", Role.Estudiante, "Luis");
        users.Save(byEmail);
        users.Save(byId);

        var result = useCase.Execute(new AddStudentsToCourseCommand(course.Id, [byEmail.Email.Value, byId.Id.ToString()]));

        Assert.All(result.Outcomes, o => Assert.True(o.Success));
        Assert.Equal(2, enrollments.SavedEnrollments.Count);
    }

    [Fact]
    public void Execute_WithUnknownIdentifier_ReportsFailureWithoutStoppingOthers()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var student = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        users.Save(student);

        var result = useCase.Execute(new AddStudentsToCourseCommand(course.Id, [student.Email.Value, "no-existe@sena.edu.co"]));

        Assert.Equal(2, result.Outcomes.Count);
        Assert.True(result.Outcomes.Single(o => o.Identifier == student.Email.Value).Success);
        Assert.False(result.Outcomes.Single(o => o.Identifier == "no-existe@sena.edu.co").Success);
        Assert.Single(enrollments.SavedEnrollments);
    }

    [Fact]
    public void Execute_WithNonStudentRole_ReportsFailure()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.ConAprobacion, Guid.NewGuid());
        courses.Save(course);
        var teacher = User.Register(Email.Create("profe@sena.edu.co"), "hash", Role.Profesor, "Profe");
        users.Save(teacher);

        var result = useCase.Execute(new AddStudentsToCourseCommand(course.Id, [teacher.Email.Value]));

        Assert.False(Assert.Single(result.Outcomes).Success);
    }

    [Fact]
    public void Execute_WhenAlreadyEnrolled_ReportsFailure()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var student = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        users.Save(student);
        enrollments.Save(Enrollment.Enroll(course.Id, student.Id));

        var result = useCase.Execute(new AddStudentsToCourseCommand(course.Id, [student.Email.Value]));

        Assert.False(Assert.Single(result.Outcomes).Success);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var (_, _, _, useCase) = CreateSut();

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AddStudentsToCourseCommand(Guid.NewGuid(), ["a@b.com"])));
    }

    [Fact]
    public void Execute_WhenCourseArchived_Throws()
    {
        var (courses, _, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        course.Archive();
        courses.Save(course);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new AddStudentsToCourseCommand(course.Id, ["a@b.com"])));
    }
}
