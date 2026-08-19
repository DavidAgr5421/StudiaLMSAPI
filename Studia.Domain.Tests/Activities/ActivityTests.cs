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

    [Fact]
    public void Create_Grupal_WithoutCohortIds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(Guid.NewGuid(), "Trabajo en equipo", "", DueDate, ActivityType.SoloTexto, maxFiles: null, kind: ActivityKind.Grupal));
    }

    [Fact]
    public void Create_Grupal_WithCohortIds_Succeeds()
    {
        var cohortId = Guid.NewGuid();

        var activity = Activity.Create(
            Guid.NewGuid(), "Trabajo en equipo", "", DueDate, ActivityType.SoloTexto, maxFiles: null, cohortIds: [cohortId], kind: ActivityKind.Grupal);

        Assert.Equal(ActivityKind.Grupal, activity.Kind);
        Assert.Contains(cohortId, activity.CohortIds);
    }

    [Fact]
    public void Create_WithOpenDateAfterDueDate_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Activity.Create(
                Guid.NewGuid(), "Ensayo", "", DueDate, ActivityType.SoloTexto, maxFiles: null, openDateUtc: DueDate.AddDays(1)));
    }

    [Fact]
    public void HasOpenedAt_WithoutOpenDate_IsAlwaysTrue()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, maxFiles: null);

        Assert.True(activity.HasOpenedAt(DateTime.UtcNow));
    }

    [Fact]
    public void HasOpenedAt_BeforeOpenDate_IsFalse()
    {
        var activity = Activity.Create(
            Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(2), ActivityType.SoloTexto, maxFiles: null,
            openDateUtc: DateTime.UtcNow.AddDays(1));

        Assert.False(activity.HasOpenedAt(DateTime.UtcNow));
    }

    [Fact]
    public void HasOpenedAt_AfterOpenDate_IsTrue()
    {
        var activity = Activity.Create(
            Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(2), ActivityType.SoloTexto, maxFiles: null,
            openDateUtc: DateTime.UtcNow.AddDays(-1));

        Assert.True(activity.HasOpenedAt(DateTime.UtcNow));
    }

    [Fact]
    public void AcceptsSubmissionsAt_BeforeDueDate_IsTrue()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, maxFiles: null);

        Assert.True(activity.AcceptsSubmissionsAt(DateTime.UtcNow));
    }

    [Fact]
    public void AcceptsSubmissionsAt_AfterDueDateWithLateAllowed_IsTrue()
    {
        var activity = Activity.Create(
            Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(-1), ActivityType.SoloTexto, maxFiles: null, allowsLateSubmission: true);

        Assert.True(activity.AcceptsSubmissionsAt(DateTime.UtcNow));
    }

    [Fact]
    public void AcceptsSubmissionsAt_AfterDueDateWithLateDisallowed_IsFalse()
    {
        var activity = Activity.Create(
            Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(-1), ActivityType.SoloTexto, maxFiles: null, allowsLateSubmission: false);

        Assert.False(activity.AcceptsSubmissionsAt(DateTime.UtcNow));
    }

    [Fact]
    public void Close_BeforeDueDate_BlocksSubmissions()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, maxFiles: null);

        activity.Close();

        Assert.False(activity.AcceptsSubmissionsAt(DateTime.UtcNow));
    }

    [Fact]
    public void Close_WhenAlreadyClosed_Throws()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DueDate, ActivityType.SoloTexto, maxFiles: null);
        activity.Close();

        Assert.Throws<InvalidOperationException>(() => activity.Close());
    }

    [Fact]
    public void Reopen_WhenNotClosed_Throws()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DueDate, ActivityType.SoloTexto, maxFiles: null);

        Assert.Throws<InvalidOperationException>(() => activity.Reopen());
    }

    [Fact]
    public void Reopen_AfterClose_RestoresSubmissions()
    {
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, maxFiles: null);
        activity.Close();

        activity.Reopen();

        Assert.True(activity.AcceptsSubmissionsAt(DateTime.UtcNow));
    }
}
