using Studia.Application.Courses;
using Studia.Application.Tests.Cohorts;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Courses;

public class SearchCoursesUseCaseTests
{
    [Fact]
    public void Execute_MatchesByCourseName()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var useCase = new SearchCoursesUseCase(courses, new FakeCohortRepository());

        var results = useCase.Execute(new SearchCoursesQuery("english"));

        Assert.Single(results);
    }

    [Fact]
    public void Execute_MatchesByCohortName_ReturnsParentCourse()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);

        var cohorts = new FakeCohortRepository();
        cohorts.Save(Cohort.Create(course.Id, "Ficha 123456"));

        var useCase = new SearchCoursesUseCase(courses, cohorts);

        var results = useCase.Execute(new SearchCoursesQuery("123456"));

        var result = Assert.Single(results);
        Assert.Equal(course.Id, result.Id);
    }

    [Fact]
    public void Execute_MatchingBothCourseAndCohort_DoesNotDuplicateCourse()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("Nivelacion A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);

        var cohorts = new FakeCohortRepository();
        cohorts.Save(Cohort.Create(course.Id, "Nivelacion Ficha 1"));

        var useCase = new SearchCoursesUseCase(courses, cohorts);

        var results = useCase.Execute(new SearchCoursesQuery("nivelacion"));

        Assert.Single(results);
    }

    [Fact]
    public void Execute_WithNoMatches_ReturnsEmpty()
    {
        var useCase = new SearchCoursesUseCase(new FakeCourseRepository(), new FakeCohortRepository());

        var results = useCase.Execute(new SearchCoursesQuery("no-existe"));

        Assert.Empty(results);
    }
}
