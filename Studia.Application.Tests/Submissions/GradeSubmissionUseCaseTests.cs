using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Users;
using Studia.Application.Submissions;
using Studia.Domain.Activities;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Submissions;

public class GradeSubmissionUseCaseTests
{
    private static GradeSubmissionUseCase CreateUseCase(FakeSubmissionRepository repository) =>
        new(repository, new FakeActivityRepository(), new FakeUserRepository(), new FakeNotificationRepository(), new FakeEmailSender());

    [Fact]
    public void Execute_WithValidScore_GradesSubmission()
    {
        var repository = new FakeSubmissionRepository();
        var submission = Submission.SubmitText(Guid.NewGuid(), Guid.NewGuid(), "Respuesta", DateTime.UtcNow.AddDays(1));
        repository.Save(submission);
        var useCase = CreateUseCase(repository);

        var result = useCase.Execute(new GradeSubmissionCommand(submission.Id, 5, "Excelente"));

        Assert.Equal(5, result.Score);
        Assert.Equal("Excelente", result.Feedback);
    }

    [Fact]
    public void Execute_NotifiesTheStudent()
    {
        var repository = new FakeSubmissionRepository();
        var student = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante, "Ana Gómez");
        var users = new FakeUserRepository();
        users.Save(student);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", DateTime.UtcNow.AddDays(1), ActivityType.SoloTexto, null);
        activities.Save(activity);

        var submission = Submission.SubmitText(activity.Id, student.Id, "Respuesta", activity.DueDateUtc);
        repository.Save(submission);

        var notifications = new FakeNotificationRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new GradeSubmissionUseCase(repository, activities, users, notifications, emailSender);

        useCase.Execute(new GradeSubmissionCommand(submission.Id, 5, "Excelente"));

        var notification = Assert.Single(notifications.SavedNotifications);
        Assert.Equal(student.Id, notification.RecipientUserId);
        Assert.Single(emailSender.SentEmails);
    }

    [Fact]
    public void Execute_WhenSubmissionDoesNotExist_Throws()
    {
        var useCase = CreateUseCase(new FakeSubmissionRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new GradeSubmissionCommand(Guid.NewGuid(), 5, null)));
    }
}
