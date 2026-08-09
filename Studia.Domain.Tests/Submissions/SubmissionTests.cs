using Studia.Domain.Submissions;

namespace Studia.Domain.Tests.Submissions;

public class SubmissionTests
{
    [Fact]
    public void SubmitText_BeforeDueDate_IsOnTime()
    {
        var dueDate = DateTime.UtcNow.AddDays(1);

        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Mi respuesta", dueDate);

        Assert.Equal(SubmissionStatus.ATiempo, submission.Status);
        Assert.Equal("Mi respuesta", submission.TextContent);
        Assert.Empty(submission.Files);
    }

    [Fact]
    public void SubmitText_AfterDueDate_IsLate()
    {
        var dueDate = DateTime.UtcNow.AddDays(-1);

        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Mi respuesta", dueDate);

        Assert.Equal(SubmissionStatus.Tardia, submission.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SubmitText_WithBlankContent_Throws(string blankContent)
    {
        Assert.Throws<ArgumentException>(() =>
            Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), blankContent, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void SubmitWithFiles_WithinLimit_Succeeds()
    {
        var files = new[] { SubmittedFile.Create("a.pdf", "key1", 1024) };

        var submission = Submission.SubmitWithFiles(Guid.NewGuid(), Guid.NewGuid(), files, maxFiles: 2, DateTime.UtcNow.AddDays(1));

        Assert.Single(submission.Files);
        Assert.Null(submission.TextContent);
    }

    [Fact]
    public void SubmitWithFiles_ExceedingMaxFiles_Throws()
    {
        var files = new[]
        {
            SubmittedFile.Create("a.pdf", "key1", 1024),
            SubmittedFile.Create("b.pdf", "key2", 1024)
        };

        Assert.Throws<ArgumentException>(() =>
            Submission.SubmitWithFiles(Guid.NewGuid(), Guid.NewGuid(), files, maxFiles: 1, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void SubmitWithFiles_WithoutFiles_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Submission.SubmitWithFiles(Guid.NewGuid(), Guid.NewGuid(), [], maxFiles: 2, DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Grade_WithValidScore_SetsScoreAndFeedback()
    {
        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Respuesta", DateTime.UtcNow.AddDays(1));

        submission.Grade(85, "Buen trabajo");

        Assert.Equal(85, submission.Score);
        Assert.Equal("Buen trabajo", submission.Feedback);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Grade_WithOutOfRangeScore_Throws(int invalidScore)
    {
        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Respuesta", DateTime.UtcNow.AddDays(1));

        Assert.Throws<ArgumentException>(() => submission.Grade(invalidScore, null));
    }
}
