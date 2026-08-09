using Studia.Application.Sections;
using Studia.Application.Tests.Courses;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Sections;

public class CreateSectionUseCaseTests
{
    [Fact]
    public void Execute_WithActiveCourse_SavesSanitizedSection()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        courses.Save(course);
        var sections = new FakeSectionRepository();
        var useCase = new CreateSectionUseCase(sections, courses, new FakeHtmlSanitizer());

        var result = useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "<script>alert(1)</script>"));

        Assert.Equal("sanitized:<script>alert(1)</script>", result.DescriptionHtml);
        Assert.Single(sections.SavedSections);
    }

    [Fact]
    public void Execute_WhenCourseDoesNotExist_Throws()
    {
        var useCase = new CreateSectionUseCase(new FakeSectionRepository(), new FakeCourseRepository(), new FakeHtmlSanitizer());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateSectionCommand(Guid.NewGuid(), "Semana 1", "")));
    }

    [Fact]
    public void Execute_WhenCourseIsArchived_Throws()
    {
        var courses = new FakeCourseRepository();
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        course.Archive();
        courses.Save(course);
        var useCase = new CreateSectionUseCase(new FakeSectionRepository(), courses, new FakeHtmlSanitizer());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateSectionCommand(course.Id, "Semana 1", "")));
    }
}
