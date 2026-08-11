using Studia.Application.Sections;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Submissions;
using Studia.Domain.Activities;
using Studia.Domain.Sections;
using Studia.Domain.Submissions;

namespace Studia.Application.Tests.Sections;

public class DeleteSectionUseCaseTests
{
    [Fact]
    public void Execute_DeletesSectionAndItsActivitiesAndSubmissions()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(section.Id, "Tarea", "", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activities.Save(activity);

        var submissions = new FakeSubmissionRepository();
        var submission = Submission.SubmitText(activity.Id, Guid.NewGuid(), "Mi respuesta", DateTime.UtcNow.AddDays(1));
        submissions.Save(submission);

        var useCase = new DeleteSectionUseCase(sections, activities, submissions);

        useCase.Execute(new DeleteSectionCommand(section.Id));

        Assert.Null(sections.GetById(section.Id));
        Assert.Empty(activities.GetBySectionId(section.Id));
        Assert.Empty(submissions.GetByActivityId(activity.Id));
    }

    [Fact]
    public void Execute_WhenSectionDoesNotExist_Throws()
    {
        var useCase = new DeleteSectionUseCase(new FakeSectionRepository(), new FakeActivityRepository(), new FakeSubmissionRepository());

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new DeleteSectionCommand(Guid.NewGuid())));
    }
}
