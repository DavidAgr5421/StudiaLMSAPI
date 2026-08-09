using Studia.Domain.Sections;

namespace Studia.Domain.Tests.Sections;

public class SectionTests
{
    [Fact]
    public void Create_WithValidData_SetsFields()
    {
        var courseId = Guid.NewGuid();

        var section = Section.Create(courseId, "Semana 1", "<p>Bienvenidos</p>");

        Assert.Equal(courseId, section.CourseId);
        Assert.Equal("Semana 1", section.Title);
        Assert.Equal("<p>Bienvenidos</p>", section.DescriptionHtml);
    }

    [Fact]
    public void Create_WithEmptyCourseId_Throws()
    {
        Assert.Throws<ArgumentException>(() => Section.Create(Guid.Empty, "Semana 1", ""));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_Throws(string blankTitle)
    {
        Assert.Throws<ArgumentException>(() => Section.Create(Guid.NewGuid(), blankTitle, ""));
    }

    [Fact]
    public void Create_WithTitleLongerThan150Characters_Throws()
    {
        var tooLong = new string('a', 151);

        Assert.Throws<ArgumentException>(() => Section.Create(Guid.NewGuid(), tooLong, ""));
    }

    [Fact]
    public void Create_WithEmptyDescription_IsAllowed()
    {
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");

        Assert.Equal("", section.DescriptionHtml);
    }
}
