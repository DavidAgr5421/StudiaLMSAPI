using Studia.Application.Activities;
using Studia.Application.Tests.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Sections;

namespace Studia.Application.Tests.Activities;

public class CreateActivityUseCaseTests
{
    private static readonly DateTime DueDate = new(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Execute_WithExistingSection_SavesAndReturnsActivity()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var activities = new FakeActivityRepository();
        var useCase = new CreateActivityUseCase(activities, sections);

        var result = useCase.Execute(new CreateActivityCommand(section.Id, "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null));

        Assert.Single(activities.SavedActivities);
        Assert.Equal(section.Id, result.SectionId);
    }

    [Fact]
    public void Execute_WhenSectionDoesNotExist_Throws()
    {
        var useCase = new CreateActivityUseCase(new FakeActivityRepository(), new FakeSectionRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new CreateActivityCommand(Guid.NewGuid(), "Tarea", "Descripción", DueDate, ActivityType.SoloTexto, null)));
    }

    [Fact]
    public void Execute_PropagatesDomainValidationForFileActivityWithoutMaxFiles()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var useCase = new CreateActivityUseCase(new FakeActivityRepository(), sections);

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(new CreateActivityCommand(section.Id, "Tarea", "Descripción", DueDate, ActivityType.ConArchivo, null)));
    }
}
