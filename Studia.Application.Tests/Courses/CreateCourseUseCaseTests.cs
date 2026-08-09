using Studia.Application.Courses;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Courses;

public class CreateCourseUseCaseTests
{
    [Fact]
    public void Execute_WithValidCommand_SavesAndReturnsCourse()
    {
        var repository = new FakeCourseRepository();
        var useCase = new CreateCourseUseCase(repository);

        var result = useCase.Execute(new CreateCourseCommand("English A1", EnrollmentMode.Abierta));

        var savedCourse = Assert.Single(repository.SavedCourses);
        Assert.Equal(result.Id, savedCourse.Id);
        Assert.Equal("English A1", savedCourse.Name);
    }

    [Fact]
    public void Execute_WithBlankName_PropagatesDomainValidation()
    {
        var useCase = new CreateCourseUseCase(new FakeCourseRepository());

        Assert.Throws<ArgumentException>(() => useCase.Execute(new CreateCourseCommand("", EnrollmentMode.Abierta)));
    }
}
