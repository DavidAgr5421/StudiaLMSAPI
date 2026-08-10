using Studia.Application.Activities;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Cohorts;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Activities;

public class GetActivitiesBySectionUseCaseTests
{
    private static GetActivitiesBySectionUseCase CreateSut(
        FakeActivityRepository activities,
        FakeSectionRepository? sections = null,
        FakeCohortRepository? cohorts = null) =>
        new(activities, sections ?? new FakeSectionRepository(), cohorts ?? new FakeCohortRepository());

    [Fact]
    public void Execute_ReturnsOnlyActivitiesOfThatSection()
    {
        var repository = new FakeActivityRepository();
        var sectionId = Guid.NewGuid();
        var matching = Activity.Create(sectionId, "Tarea", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        var other = Activity.Create(Guid.NewGuid(), "Otra", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        repository.Save(matching);
        repository.Save(other);
        var useCase = CreateSut(repository);

        var results = useCase.Execute(new GetActivitiesBySectionQuery(sectionId, Guid.NewGuid(), Role.Profesor));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoActivities_ReturnsEmpty()
    {
        var useCase = CreateSut(new FakeActivityRepository());

        var results = useCase.Execute(new GetActivitiesBySectionQuery(Guid.NewGuid(), Guid.NewGuid(), Role.Profesor));

        Assert.Empty(results);
    }

    [Fact]
    public void Execute_AsStudent_SeesGlobalAndOwnCohortActivities_ButNotOthers()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);

        var cohorts = new FakeCohortRepository();
        var myCohort = Cohort.Create(section.CourseId, "Ficha A");
        var studentId = Guid.NewGuid();
        myCohort.AssignStudent(studentId);
        var otherCohort = Cohort.Create(section.CourseId, "Ficha B");
        cohorts.Save(myCohort);
        cohorts.Save(otherCohort);

        var activities = new FakeActivityRepository();
        var global = Activity.Create(section.Id, "Global", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        var forMyCohort = Activity.Create(section.Id, "Mi ficha", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null, [myCohort.Id]);
        var forOtherCohort = Activity.Create(section.Id, "Otra ficha", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null, [otherCohort.Id]);
        activities.Save(global);
        activities.Save(forMyCohort);
        activities.Save(forOtherCohort);

        var useCase = CreateSut(activities, sections, cohorts);

        var results = useCase.Execute(new GetActivitiesBySectionQuery(section.Id, studentId, Role.Estudiante));

        var resultIds = results.Select(r => r.Id).ToHashSet();
        Assert.Equal(2, resultIds.Count);
        Assert.Contains(global.Id, resultIds);
        Assert.Contains(forMyCohort.Id, resultIds);
        Assert.DoesNotContain(forOtherCohort.Id, resultIds);
    }

    [Fact]
    public void Execute_AsStudentNotInSectionCohort_ReturnsEmpty()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        var cohorts = new FakeCohortRepository();
        var restrictedCohort = Cohort.Create(section.CourseId, "Ficha A");
        var restrictedSection = Section.Create(section.CourseId, "Solo ficha A", "", [restrictedCohort.Id]);
        sections.Save(restrictedSection);
        cohorts.Save(restrictedCohort);

        var activities = new FakeActivityRepository();
        activities.Save(Activity.Create(restrictedSection.Id, "Tarea", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null));

        var useCase = CreateSut(activities, sections, cohorts);

        var results = useCase.Execute(new GetActivitiesBySectionQuery(restrictedSection.Id, Guid.NewGuid(), Role.Estudiante));

        Assert.Empty(results);
    }
}
