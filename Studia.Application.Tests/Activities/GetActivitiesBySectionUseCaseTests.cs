using Studia.Application.Activities;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Sections;
using Studia.Domain.Activities;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Activities;

public class GetActivitiesBySectionUseCaseTests
{
    private static GetActivitiesBySectionUseCase CreateSut(
        FakeActivityRepository activities,
        FakeSectionRepository? sections = null,
        FakeCohortRepository? cohorts = null) =>
        new(activities, sections ?? new FakeSectionRepository(), cohorts ?? new FakeCohortRepository(), new FakeCourseRepository());

    [Fact]
    public void Execute_ReturnsOnlyActivitiesOfThatSection()
    {
        var repository = new FakeActivityRepository();
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);
        var matching = Activity.Create(section.Id, "Tarea", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        var other = Activity.Create(Guid.NewGuid(), "Otra", "Descripción", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        repository.Save(matching);
        repository.Save(other);
        var useCase = CreateSut(repository, sections);

        var results = useCase.Execute(new GetActivitiesBySectionQuery(section.Id, Guid.NewGuid(), Role.Profesor));

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

    [Fact]
    public void Execute_HiddenActivity_IsInvisibleToOtherProfesor_ButVisibleToOwner()
    {
        var profesorId = Guid.NewGuid();
        var courses = new FakeCourseRepository();
        var course = Course.Create("Curso", EnrollmentMode.Abierta, profesorId);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var visible = Activity.Create(section.Id, "Tarea 1", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        var hidden = Activity.Create(
            section.Id, "Borrador", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null, status: ActivityStatus.Oculto);
        activities.Save(visible);
        activities.Save(hidden);

        var useCase = new GetActivitiesBySectionUseCase(activities, sections, new FakeCohortRepository(), courses);

        var asOtherProfesor = useCase.Execute(new GetActivitiesBySectionQuery(section.Id, Guid.NewGuid(), Role.Profesor));
        var asOwner = useCase.Execute(new GetActivitiesBySectionQuery(section.Id, profesorId, Role.Profesor));

        Assert.DoesNotContain(asOtherProfesor, r => r.Id == hidden.Id);
        Assert.Contains(asOwner, r => r.Id == hidden.Id);
    }
}
