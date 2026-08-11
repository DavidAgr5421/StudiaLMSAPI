using Studia.Application.Courses;
using Studia.Application.Tests.Users;
using Studia.Domain.Courses;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Courses;

public class GetCourseByIdUseCaseTests
{
    [Fact]
    public void Execute_WithExistingCourse_ReturnsResult()
    {
        var repository = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        repository.Save(course);
        var useCase = new GetCourseByIdUseCase(repository, new FakeUserRepository());

        var result = useCase.Execute(new GetCourseByIdQuery(course.Id));

        Assert.NotNull(result);
        Assert.Equal(course.Id, result!.Id);
    }

    [Fact]
    public void Execute_IncludesProfesorName()
    {
        var repository = new FakeCourseRepository();
        var profesor = User.Register(Email.Create("prof@sena.edu.co"), "hash", Role.Profesor, "Juan Pérez");
        var course = Course.Create("English A1", EnrollmentMode.Abierta, profesor.Id);
        repository.Save(course);

        var users = new FakeUserRepository();
        users.Save(profesor);

        var useCase = new GetCourseByIdUseCase(repository, users);

        var result = useCase.Execute(new GetCourseByIdQuery(course.Id));

        Assert.Equal("Juan Pérez", result!.ProfesorName);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_ReturnsNull()
    {
        var useCase = new GetCourseByIdUseCase(new FakeCourseRepository(), new FakeUserRepository());

        var result = useCase.Execute(new GetCourseByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }
}
