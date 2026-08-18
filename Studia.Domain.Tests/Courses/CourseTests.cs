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

    [Fact]
    public void UpdateColor_WithValidHex_SetsColor()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());

        course.UpdateColor("#7C3AED");

        Assert.Equal("#7C3AED", course.Color);
    }

    [Fact]
    public void UpdateColor_WithNull_ClearsColor()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        course.UpdateColor("#7C3AED");

        course.UpdateColor(null);

        Assert.Null(course.Color);
    }

    [Theory]
    [InlineData("7C3AED")]
    [InlineData("#7C3AE")]
    [InlineData("#GGGGGG")]
    [InlineData("red")]
    public void UpdateColor_WithInvalidFormat_Throws(string invalidColor)
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => course.UpdateColor(invalidColor));
    }

    [Fact]
    public void SetCoverImage_WithValidData_SetsFields()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());

        course.SetCoverImage("portada.png", "storage-key-123", 1024);

        Assert.Equal("portada.png", course.CoverImageFileName);
        Assert.Equal("storage-key-123", course.CoverImageStorageKey);
        Assert.Equal(1024, course.CoverImageSizeBytes);
    }

    [Fact]
    public void SetCoverImage_TooLarge_Throws()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => course.SetCoverImage("portada.png", "key", Course.MaxCoverImageSizeBytes + 1));
    }

    [Fact]
    public void RemoveCoverImage_ClearsFields()
    {
        var course = Course.Create("English A1", EnrollmentMode.Abierta, Guid.NewGuid());
        course.SetCoverImage("portada.png", "storage-key-123", 1024);

        course.RemoveCoverImage();

        Assert.Null(course.CoverImageFileName);
        Assert.Null(course.CoverImageStorageKey);
        Assert.Null(course.CoverImageSizeBytes);
    }
}
