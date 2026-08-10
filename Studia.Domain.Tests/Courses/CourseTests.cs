using Studia.Domain.Courses;

namespace Studia.Domain.Tests.Courses;

public class CourseTests
{
    [Fact]
    public void Create_WithValidData_StartsActive()
    {
        var profesorId = Guid.NewGuid();
        var course = Course.Create("English A1", EnrollmentMode.Abierta, profesorId);

        Assert.Equal("English A1", course.Name);
        Assert.Equal(EnrollmentMode.Abierta, course.EnrollmentMode);
        Assert.Equal(CourseStatus.Activo, course.Status);
        Assert.Equal(profesorId, course.ProfesorId);
        Assert.NotEqual(Guid.Empty, course.Id);
    }

    [Fact]
    public void Create_WithEmptyProfesorId_Throws()
    {
        Assert.Throws<ArgumentException>(() => Course.Create("English A1", EnrollmentMode.Abierta, Guid.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string blankName)
    {
        Assert.Throws<ArgumentException>(() => Course.Create(blankName, EnrollmentMode.Abierta, Guid.NewGuid()));
    }

    [Fact]
    public void Create_WithNameLongerThan150Characters_Throws()
    {
        var tooLong = new string('a', 151);

        Assert.Throws<ArgumentException>(() => Course.Create(tooLong, EnrollmentMode.Abierta, Guid.NewGuid()));
    }

    [Fact]
    public void Archive_WhenActive_SetsStatusToArchivado()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());

        course.Archive();

        Assert.Equal(CourseStatus.Archivado, course.Status);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_Throws()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        course.Archive();

        Assert.Throws<InvalidOperationException>(() => course.Archive());
    }

    // El código de invitación es independiente del modo de inscripción: cualquier curso
    // se puede compartir por invitación, sea de auto-servicio o de aprobación manual.
    [Theory]
    [InlineData(EnrollmentMode.Abierta)]
    [InlineData(EnrollmentMode.ConAprobacion)]
    public void Create_AlwaysGeneratesInvitationCode(EnrollmentMode enrollmentMode)
    {
        var course = Course.Create("English A1", enrollmentMode, Guid.NewGuid());

        Assert.NotNull(course.InvitationCode);
        Assert.Equal(8, course.InvitationCode.Length);
    }

    [Fact]
    public void Create_GeneratesDifferentCodesEachTime()
    {
        var first = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        var second = Course.Create("English A2", EnrollmentMode.Abierta, Guid.NewGuid());

        Assert.NotEqual(first.InvitationCode, second.InvitationCode);
    }
}
