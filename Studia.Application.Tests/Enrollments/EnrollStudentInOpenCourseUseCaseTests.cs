using Studia.Application.Enrollments;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Users;
using Studia.Domain.Courses;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Enrollments;

public class EnrollStudentInOpenCourseUseCaseTests
{
    private static (FakeCourseRepository Courses, FakeUserRepository Users, FakeEnrollmentRepository Enrollments, EnrollStudentInOpenCourseUseCase UseCase) CreateSut()
    {
        var courses = new FakeCourseRepository();
        var users = new FakeUserRepository();
        var enrollments = new FakeEnrollmentRepository();
        var useCase = new EnrollStudentInOpenCourseUseCase(enrollments, courses, users);

        return (courses, users, enrollments, useCase);
    }

    private static User CreateStudent(string email = "estudiante@sena.edu.co") =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante);

    [Fact]
    public void Execute_WithOpenActiveCourseAndStudent_EnrollsSuccessfully()
    {
        var (courses, users, enrollments, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        var result = useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, student.Id));

        var saved = Assert.Single(enrollments.SavedEnrollments);
        Assert.Equal(result.Id, saved.Id);
        Assert.Equal(course.Id, saved.CourseId);
        Assert.Equal(student.Id, saved.StudentId);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var (_, users, _, useCase) = CreateSut();
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(Guid.NewGuid(), student.Id)));
    }

    [Fact]
    public void Execute_WhenCourseIsNotOpenEnrollment_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English B1", EnrollmentMode.ConAprobacion);
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, student.Id)));
    }

    [Fact]
    public void Execute_WhenCourseIsArchived_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        course.Archive();
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, student.Id)));
    }

    [Fact]
    public void Execute_WhenStudentDoesNotExist_Throws()
    {
        var (courses, _, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courses.Save(course);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, Guid.NewGuid())));
    }

    [Fact]
    public void Execute_WhenUserIsNotAStudent_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courses.Save(course);
        var teacher = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor);
        users.Save(teacher);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, teacher.Id)));
    }

    [Fact]
    public void Execute_WhenStudentAlreadyEnrolled_Throws()
    {
        var (courses, users, _, useCase) = CreateSut();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courses.Save(course);
        var student = CreateStudent();
        users.Save(student);
        useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, student.Id));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new EnrollStudentInOpenCourseCommand(course.Id, student.Id)));
    }
}
