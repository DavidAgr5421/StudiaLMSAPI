using Studia.Domain.Courses;

namespace Studia.Domain.Tests.Courses;

public class CourseTests
{
    [Fact]
    public void Create_WithValidData_StartsActive()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta);

        Assert.Equal("English A1", course.Name);
        Assert.Equal(EnrollmentMode.Abierta, course.EnrollmentMode);
        Assert.Equal(CourseStatus.Activo, course.Status);
        Assert.NotEqual(Guid.Empty, course.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string blankName)
    {
        Assert.Throws<ArgumentException>(() => Course.Create(blankName, EnrollmentMode.Abierta));
    }

    [Fact]
    public void Create_WithNameLongerThan150Characters_Throws()
    {
        var tooLong = new string('a', 151);

        Assert.Throws<ArgumentException>(() => Course.Create(tooLong, EnrollmentMode.Abierta));
    }

    [Fact]
    public void Archive_WhenActive_SetsStatusToArchivado()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta);

        course.Archive();

        Assert.Equal(CourseStatus.Archivado, course.Status);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_Throws()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta);
        course.Archive();

        Assert.Throws<InvalidOperationException>(() => course.Archive());
    }

    [Fact]
    public void Create_WithInvitationMode_GeneratesInvitationCode()
    {
        var course = Course.Create("English A1", EnrollmentMode.PorInvitacion);

        Assert.NotNull(course.InvitationCode);
        Assert.Equal(8, course.InvitationCode.Length);
    }

    [Theory]
    [InlineData(EnrollmentMode.Abierta)]
    [InlineData(EnrollmentMode.ConAprobacion)]
    public void Create_WithoutInvitationMode_LeavesInvitationCodeNull(EnrollmentMode enrollmentMode)
    {
        var course = Course.Create("English A1", enrollmentMode);

        Assert.Null(course.InvitationCode);
    }

    [Fact]
    public void Create_WithInvitationMode_GeneratesDifferentCodesEachTime()
    {
        var first = Course.Create("English A1", EnrollmentMode.PorInvitacion);
        var second = Course.Create("English A2", EnrollmentMode.PorInvitacion);

        Assert.NotEqual(first.InvitationCode, second.InvitationCode);
    }
}
