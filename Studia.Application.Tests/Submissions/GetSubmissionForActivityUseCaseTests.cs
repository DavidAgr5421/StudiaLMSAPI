using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Application.Tests.Submissions;

public class GetSubmissionForActivityUseCaseTests
{
    [Fact]
    public void Execute_WhenStudentSubmitted_ReturnsTheirSubmission()
    {
        var repository = new FakeSubmissionRepository();
        var activityId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var mine = Submission.SubmitText(activityId, studentId, "Respuesta", DateTime.UtcNow.AddDays(1));
        var classmate = Submission.SubmitText(activityId, Guid.NewGuid(), "Otra", DateTime.UtcNow.AddDays(1));
        repository.Save(mine);
        repository.Save(classmate);
        var useCase = new GetSubmissionForActivityUseCase(repository);

        var result = useCase.Execute(new GetSubmissionForActivityQuery(activityId, studentId));

        Assert.NotNull(result);
        Assert.Equal(mine.Id, result!.Id);
    }

    [Fact]
    public void Execute_WhenStudentHasNotSubmitted_ReturnsNull()
    {
        var repository = new FakeSubmissionRepository();
        var activityId = Guid.NewGuid();
        repository.Save(Submission.SubmitText(activityId, Guid.NewGuid(), "Otra", DateTime.UtcNow.AddDays(1)));
        var useCase = new GetSubmissionForActivityUseCase(repository);

        var result = useCase.Execute(new GetSubmissionForActivityQuery(activityId, Guid.NewGuid()));

        Assert.Null(result);
    }
}
