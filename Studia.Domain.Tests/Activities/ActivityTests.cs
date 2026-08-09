using Studia.Domain.Activities;

namespace Studia.Domain.Tests.Activities;

public class ActivityTests
{
    private static readonly DateTime DueDate = new(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_TextOnlyWithoutMaxFiles_Succeeds()
    {
        var sectionId = Guid.NewGuid();

        var activity = Activity.Create(sectionId, "Ensayo", "Escriba 200 palabras", DueDate, ActivityType.SoloTexto, maxFiles: null);

        Assert.Equal(sectionId, activity.SectionId);
        Assert.Equal(ActivityType.SoloTexto, activity.Type);
        Assert.Null(activity.MaxFiles);
    }

    [Fact]
    public void Create_WithFilesAndPositiveMaxFiles_Succeeds()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", DueDate, ActivityType.ConArchivo, maxFiles: 3);

        Assert.Equal(3, activity.MaxFiles);
    }

    [Fact]
    public void Create_WithFilesAndNullMaxFiles_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", DueDate, ActivityType.ConArchivo, maxFiles: null));
    }

    [Fact]
    public void Create_WithFilesAndZeroMaxFiles_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", DueDate, ActivityType.ConArchivo, maxFiles: 0));
    }

    [Fact]
    public void Create_TextOnlyWithMaxFiles_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", DueDate, ActivityType.SoloTexto, maxFiles: 2));
    }

    [Fact]
    public void Create_WithEmptySectionId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.Empty, "Ensayo", "Escriba 200 palabras", DueDate, ActivityType.SoloTexto, maxFiles: null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_Throws(string blankTitle)
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.NewGuid(), blankTitle, "Descripción", DueDate, ActivityType.SoloTexto, maxFiles: null));
    }
}
