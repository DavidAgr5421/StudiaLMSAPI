using Studia.Application.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Cohorts;

public class CreateCohortUseCaseTests
{
    [Fact]
    public void Execute_WhenCourseExists_SavesAndReturnsCohort()
    {
        var courseRepository = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courseRepository.Save(course);

        var cohortRepository = new FakeCohortRepository();
        var useCase = new CreateCohortUseCase(cohortRepository, courseRepository);

        var result = useCase.Execute(new CreateCohortCommand(course.Id, "Ficha 123456"));

        var saved = Assert.Single(cohortRepository.SavedCohorts);
        Assert.Equal(result.Id, saved.Id);
        Assert.Equal(course.Id, saved.CourseId);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var useCase = new CreateCohortUseCase(new FakeCohortRepository(), new FakeCourseRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateCohortCommand(Guid.NewGuid(), "Ficha 123456")));
    }
}
