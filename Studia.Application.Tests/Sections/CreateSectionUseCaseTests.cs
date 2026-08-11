using Studia.Application.Sections;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Sections;

public class CreateSectionUseCaseTests
{
    private static CreateSectionUseCase CreateSut(
        FakeSectionRepository sections,
        FakeCourseRepository courses,
        FakeCohortRepository? cohorts = null) =>
        new(sections, courses, cohorts ?? new FakeCohortRepository(), new FakeHtmlSanitizer());

    [Fact]
    public void Execute_WithActiveCourse_SavesSanitizedSection()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var sections = new FakeSectionRepository();
        var useCase = CreateSut(sections, courses);

        var result = useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "<script>alert(1)</script>"));

        Assert.Equal("sanitized:<script>alert(1)</script>", result.DescriptionHtml);
        Assert.Single(sections.SavedSections);
        Assert.Empty(result.CohortIds);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var useCase = CreateSut(new FakeSectionRepository(), new FakeCourseRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateSectionCommand(Guid.NewGuid(), "Semana 1", "")));
    }

    [Fact]
    public void Execute_WhenCourseIsArchived_Throws()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        course.Archive();
        courses.Save(course);
        var useCase = CreateSut(new FakeSectionRepository(), courses);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "")));
    }

    [Fact]
    public void Execute_WithCohortIdFromAnotherCourse_Throws()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var cohorts = new FakeCohortRepository();
        var foreignCohort = Cohort.Create(Guid.NewGuid(), "Ficha ajena");
        cohorts.Save(foreignCohort);
        var useCase = CreateSut(new FakeSectionRepository(), courses, cohorts);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "", [foreignCohort.Id])));
    }

    [Fact]
    public void Execute_WithValidCohortId_RestrictsSectionToThatCohort()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        courses.Save(course);
        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(course.Id, "Ficha A");
        cohorts.Save(cohort);
        var useCase = CreateSut(new FakeSectionRepository(), courses, cohorts);

        var result = useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "", [cohort.Id]));

        Assert.Equal([cohort.Id], result.CohortIds);
    }
}
