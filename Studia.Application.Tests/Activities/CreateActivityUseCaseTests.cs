using Studia.Application.Activities;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Submissions;
using Studia.Domain.Activities;
using Studia.Domain.Cohorts;
using Studia.Domain.Sections;

namespace Studia.Application.Tests.Activities;

public class CreateActivityUseCaseTests
{
    private static readonly DateTime DueDate = new(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc);

    private static CreateActivityUseCase CreateSut(
        FakeActivityRepository activities,
        FakeSectionRepository sections,
        FakeCohortRepository? cohorts = null,
        FakeFileStorage? fileStorage = null) =>
        new(activities, sections, cohorts ?? new FakeCohortRepository(), fileStorage ?? new FakeFileStorage(), new FakeHtmlSanitizer());

    [Fact]
    public void Execute_WithExistingSection_SavesAndReturnsActivity()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var activities = new FakeActivityRepository();
        var useCase = CreateSut(activities, sections);

        var result = useCase.Execute(new CreateActivityCommand(section.Id, "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null));

        Assert.Single(activities.SavedActivities);
        Assert.Equal(section.Id, result.SectionId);
    }

    [Fact]
    public void Execute_SanitizesDescriptionHtml()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var useCase = CreateSut(new FakeActivityRepository(), sections);

        var result = useCase.Execute(new CreateActivityCommand(
            section.Id, "Tarea", "<script>alert(1)</script>", DueDate, ActivityType.SoloTexto, null));

        Assert.Equal("sanitized:<script>alert(1)</script>", result.Description);
    }

    [Fact]
    public void Execute_WhenSectionDoesNotExist_Throws()
    {
        var useCase = CreateSut(new FakeActivityRepository(), new FakeSectionRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateActivityCommand(Guid.NewGuid(), "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null)));
    }

    [Fact]
    public void Execute_PropagatesDomainValidationForFileActivityWithoutMaxFiles()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var useCase = CreateSut(new FakeActivityRepository(), sections);

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(new CreateActivityCommand(section.Id, "Tarea", "Descripción", DueDate, ActivityType.ConArchivo, null)));
    }

    [Fact]
    public void Execute_WithCohortIdsFromAnotherCourse_Throws()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var cohorts = new FakeCohortRepository();
        var foreignCohort = Cohort.Create(Guid.NewGuid(), "Ficha ajena");
        cohorts.Save(foreignCohort);
        var useCase = CreateSut(new FakeActivityRepository(), sections, cohorts);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateActivityCommand(
                section.Id, "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null, [foreignCohort.Id])));
    }

    [Fact]
    public void Execute_WithBaseFiles_StoresAndReturnsThem()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var fileStorage = new FakeFileStorage();
        var useCase = CreateSut(new FakeActivityRepository(), sections, fileStorage: fileStorage);

        var result = useCase.Execute(new CreateActivityCommand(
            section.Id, "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null,
            Files: [new ActivityFileInput("guia.pdf", [1, 2, 3])]));

        var file = Assert.Single(result.Files);
        Assert.Equal("guia.pdf", file.FileName);
        Assert.Single(fileStorage.StoredFiles);
    }

    [Fact]
    public void Execute_PassesThroughKindOpenDateAndAllowsLateSubmission()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(section.CourseId, "Grupo A");
        cohorts.Save(cohort);
        var useCase = CreateSut(new FakeActivityRepository(), sections, cohorts);
        var openDate = DueDate.AddDays(-3);

        var result = useCase.Execute(new CreateActivityCommand(
            section.Id, "Trabajo en equipo", "Descripción", DueDate, ActivityType.SoloTexto, null,
            CohortIds: [cohort.Id], Kind: ActivityKind.Grupal, OpenDateUtc: openDate, AllowsLateSubmission: false));

        Assert.Equal(ActivityKind.Grupal, result.Kind);
        Assert.Equal(openDate, result.OpenDateUtc);
        Assert.False(result.AllowsLateSubmission);
    }

    [Fact]
    public void Execute_WithBaseFileOverSizeLimit_Throws()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var useCase = CreateSut(new FakeActivityRepository(), sections);
        var tooLarge = new byte[ActivityFile.MaxSizeBytes + 1];

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(new CreateActivityCommand(
                section.Id, "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null,
                Files: [new ActivityFileInput("grande.zip", tooLarge)])));
    }
}
