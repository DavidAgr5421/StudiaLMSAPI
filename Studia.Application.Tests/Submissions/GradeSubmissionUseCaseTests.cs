using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Application.Tests.Submissions;

public class GradeSubmissionUseCaseTests
{
    [Fact]
    public void Execute_WithValidScore_GradesSubmission()
    {
        var repository = new FakeSubmissionRepository();
        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Respuesta", DateTime.UtcNow.AddDays(1));
        repository.Save(submission);
        var useCase = new GradeSubmissionUseCase(repository);

        var result = useCase.Execute(new GradeSubmissionCommand(submission.Id, 90, "Excelente"));

        Assert.Equal(90, result.Score);
        Assert.Equal("Excelente", result.Feedback);
    }

    [Fact]
    public void Execute_WhenSubmissionDoesNotExist_Throws()
    {
        var useCase = new GradeSubmissionUseCase(new FakeSubmissionRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new GradeSubmissionCommand(Guid.NewGuid(), 90, null)));
    }
}
