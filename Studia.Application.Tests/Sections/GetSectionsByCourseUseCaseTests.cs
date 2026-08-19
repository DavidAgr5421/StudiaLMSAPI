using Studia.Application.Sections;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Sections;

public class GetSectionsByCourseUseCaseTests
{
    private static GetSectionsByCourseUseCase CreateSut(
        FakeSectionRepository sections, FakeCohortRepository? cohorts = null, FakeCourseRepository? courses = null) =>
        new(sections, cohorts ?? new FakeCohortRepository(), courses ?? new FakeCourseRepository());

    [Fact]
    public void Execute_ReturnsOnlySectionsOfThatCourse()
    {
        var repository = new FakeSectionRepository();
        var courseId = Guid.NewGuid();
        var matching = Section.Create(courseId, "Semana 1", "");
        var other = Section.Create(Guid.NewGuid(), "Otra", "");
        repository.Save(matching);
        repository.Save(other);
        var useCase = CreateSut(repository);

        var results = useCase.Execute(new GetSectionsByCourseQuery(courseId, Guid.NewGuid(), Role.Profesor));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoSections_ReturnsEmpty()
    {
        var useCase = CreateSut(new FakeSectionRepository());

        var results = useCase.Execute(new GetSectionsByCourseQuery(Guid.NewGuid(), Guid.NewGuid(), Role.Profesor));

        Assert.Empty(results);
    }

    [Fact]
    public void Execute_AsStudent_SeesGlobalAndOwnCohortSections_ButNotOthers()
    {
        var courseId = Guid.NewGuid();
        var cohorts = new FakeCohortRepository();
        var myCohort = Cohort.Create(courseId, "Ficha A");
        var studentId = Guid.NewGuid();
        myCohort.AssignStudent(studentId);
        var otherCohort = Cohort.Create(courseId, "Ficha B");
        cohorts.Save(myCohort);
        cohorts.Save(otherCohort);

        var sections = new FakeSectionRepository();
        var global = Section.Create(courseId, "Global", "");
        var forMyCohort = Section.Create(courseId, "Mi ficha", "", [myCohort.Id]);
        var forOtherCohort = Section.Create(courseId, "Otra ficha", "", [otherCohort.Id]);
        sections.Save(global);
        sections.Save(forMyCohort);
        sections.Save(forOtherCohort);

        var useCase = CreateSut(sections, cohorts);

        var results = useCase.Execute(new GetSectionsByCourseQuery(courseId, studentId, Role.Estudiante));

        var resultIds = results.Select(r => r.Id).ToHashSet();
        Assert.Equal(2, resultIds.Count);
        Assert.Contains(global.Id, resultIds);
        Assert.Contains(forMyCohort.Id, resultIds);
        Assert.DoesNotContain(forOtherCohort.Id, resultIds);
    }

    [Fact]
    public void Execute_HiddenSection_IsInvisibleToOtherProfesor_ButVisibleToOwner()
    {
        var profesorId = Guid.NewGuid();
        var courses = new FakeCourseRepository();
        var course = Course.Create("Curso", EnrollmentMode.Abierta, profesorId);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var visible = Section.Create(course.Id, "Semana 1", "");
        var hidden = Section.Create(course.Id, "Borrador", "", status: SectionStatus.Oculto);
        sections.Save(visible);
        sections.Save(hidden);

        var useCase = CreateSut(sections, courses: courses);

        var asOtherProfesor = useCase.Execute(new GetSectionsByCourseQuery(course.Id, Guid.NewGuid(), Role.Profesor));
        var asOwner = useCase.Execute(new GetSectionsByCourseQuery(course.Id, profesorId, Role.Profesor));

        Assert.DoesNotContain(asOtherProfesor, r => r.Id == hidden.Id);
        Assert.Contains(asOwner, r => r.Id == hidden.Id);
    }
}
