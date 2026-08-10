using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Application.Tests.Submissions;

public class GetSubmissionsByActivityUseCaseTests
{
    [Fact]
    public void Execute_ReturnsOnlySubmissionsOfThatActivity()
    {
        var repository = new FakeSubmissionRepository();
        var activityId = Guid.NewGuid();
        var matching = Submission.SubmitText(activityId, Guid.NewGuid(), "Respuesta", DateTime.UtcNow.AddDays(1));
        var other = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Otra", DateTime.UtcNow.AddDays(1));
        repository.Save(matching);
        repository.Save(other);
        var useCase = new GetSubmissionsByActivityUseCase(repository);

        var results = useCase.Execute(new GetSubmissionsByActivityQuery(activityId));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_WithNoSubmissions_ReturnsEmpty()
    {
        var useCase = new GetSubmissionsByActivityUseCase(new FakeSubmissionRepository());

        var results = useCase.Execute(new GetSubmissionsByActivityQuery(Guid.NewGuid()));

        Assert.Empty(results);
    }
}
