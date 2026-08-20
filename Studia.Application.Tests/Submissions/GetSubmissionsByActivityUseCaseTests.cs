using Studia.Application.Submissions;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Users;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

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
        var useCase = new GetSubmissionsByActivityUseCase(repository, new FakeUserRepository(), new FakeCohortRepository());

        var results = useCase.Execute(new GetSubmissionsByActivityQuery(activityId));

        var result = Assert.Single(results);
        Assert.Equal(matching.Id, result.Id);
    }

    [Fact]
    public void Execute_IncludesStudentName()
    {
        var repository = new FakeSubmissionRepository();
        var activityId = Guid.NewGuid();
        var student = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana Torres");
        var submission = Submission.SubmitText(activityId, student.Id, "Respuesta", DateTime.UtcNow.AddDays(1));
        repository.Save(submission);

        var users = new FakeUserRepository();
        users.Save(student);

        var useCase = new GetSubmissionsByActivityUseCase(repository, users, new FakeCohortRepository());

        var result = Assert.Single(useCase.Execute(new GetSubmissionsByActivityQuery(activityId)));

        Assert.Equal("Ana Torres", result.StudentName);
    }

    [Fact]
    public void Execute_WithNoSubmissions_ReturnsEmpty()
    {
        var useCase = new GetSubmissionsByActivityUseCase(new FakeSubmissionRepository(), new FakeUserRepository(), new FakeCohortRepository());

        var results = useCase.Execute(new GetSubmissionsByActivityQuery(Guid.NewGuid()));

        Assert.Empty(results);
    }
}
