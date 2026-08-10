using Studia.Application.Courses;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Courses;

public class GetCourseByIdUseCaseTests
{
    [Fact]
    public void Execute_WithExistingCourse_ReturnsResult()
    {
        var repository = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        repository.Save(course);
        var useCase = new GetCourseByIdUseCase(repository);

        var result = useCase.Execute(new GetCourseByIdQuery(course.Id));

        Assert.NotNull(result);
        Assert.Equal(course.Id, result!.Id);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_ReturnsNull()
    {
        var useCase = new GetCourseByIdUseCase(new FakeCourseRepository());

        var result = useCase.Execute(new GetCourseByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }
}
