using Studia.Application.Courses;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Users;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Courses;

public class SearchCoursesUseCaseTests
{
    private static SearchCoursesUseCase CreateUseCase(
        FakeCourseRepository? courses = null,
        FakeCohortRepository? cohorts = null,
        FakeUserRepository? users = null) =>
        new(courses ?? new FakeCourseRepository(), cohorts ?? new FakeCohortRepository(), users ?? new FakeUserRepository());

    [Fact]
    public void Execute_MatchesByCourseName()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var useCase = CreateUseCase(courses);

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

        var useCase = CreateUseCase(courses, cohorts);

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

        var useCase = CreateUseCase(courses, cohorts);

        var results = useCase.Execute(new SearchCoursesQuery("nivelacion"));

        Assert.Single(results);
    }

    [Fact]
    public void Execute_WithNoMatches_ReturnsEmpty()
    {
        var useCase = CreateUseCase();

        var results = useCase.Execute(new SearchCoursesQuery("no-existe"));

        Assert.Empty(results);
    }

    [Fact]
    public void Execute_MatchesByProfesorName()
    {
        var profesor = User.Register(Email.Create("garcia@sena.edu.co"), "hash", Role.Profesor, "Profesora García");
        var courses = new FakeCourseRepository();
        var course = Course.Create("Matemáticas II", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var users = new FakeUserRepository();
        users.Save(profesor);

        var useCase = CreateUseCase(courses, users: users);

        var results = useCase.Execute(new SearchCoursesQuery("garcía"));

        var result = Assert.Single(results);
        Assert.Equal(course.Id, result.Id);
        Assert.Equal("Profesora García", result.ProfesorName);
    }

    [Fact]
    public void Execute_MatchesByProfesorEmail()
    {
        var profesor = User.Register(Email.Create("garcia@sena.edu.co"), "hash", Role.Profesor);
        var courses = new FakeCourseRepository();
        var course = Course.Create("Matemáticas II", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var users = new FakeUserRepository();
        users.Save(profesor);

        var useCase = CreateUseCase(courses, users: users);

        var results = useCase.Execute(new SearchCoursesQuery("garcia@sena"));

        Assert.Single(results);
    }

    [Fact]
    public void Execute_WithEmptyQuery_ReturnsAllActiveCourses()
    {
        var courses = new FakeCourseRepository();
        courses.Save(Course.Create("Curso Uno", EnrollmentMode.Abierta, Guid.NewGuid()));
        courses.Save(Course.Create("Curso Dos", EnrollmentMode.ConAprobacion, Guid.NewGuid()));

        var useCase = CreateUseCase(courses);

        var results = useCase.Execute(new SearchCoursesQuery(""));

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Execute_ExcludesArchivedCourses()
    {
        var courses = new FakeCourseRepository();
        var active = Course.Create("Curso Activo", EnrollmentMode.Abierta, Guid.NewGuid());
        var archived = Course.Create("Curso Archivado", EnrollmentMode.Abierta, Guid.NewGuid());
        archived.Archive();
        courses.Save(active);
        courses.Save(archived);

        var useCase = CreateUseCase(courses);

        var results = useCase.Execute(new SearchCoursesQuery(""));

        var result = Assert.Single(results);
        Assert.Equal(active.Id, result.Id);
    }

    [Fact]
    public void Execute_WithSearchTerm_ExcludesArchivedCourses()
    {
        var courses = new FakeCourseRepository();
        var archived = Course.Create("English Archivado", EnrollmentMode.Abierta, Guid.NewGuid());
        archived.Archive();
        courses.Save(archived);

        var useCase = CreateUseCase(courses);

        var results = useCase.Execute(new SearchCoursesQuery("english"));

        Assert.Empty(results);
    }
}
