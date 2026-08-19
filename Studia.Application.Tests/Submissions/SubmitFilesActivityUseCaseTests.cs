using Studia.Application.Submissions;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Users;
using Studia.Domain.Activities;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Submissions;

public class SubmitFilesActivityUseCaseTests
{
    private static readonly DateTime FutureDueDate = DateTime.UtcNow.AddDays(1);

    private static (FakeActivityRepository Activities, FakeUserRepository Users, FakeSubmissionRepository Submissions, FakeFileStorage Storage, SubmitFilesActivityUseCase UseCase) CreateSut()
    {
        var activities = new FakeActivityRepository();
        var users = new FakeUserRepository();
        var submissions = new FakeSubmissionRepository();
        var storage = new FakeFileStorage();
        var useCase = new SubmitFilesActivityUseCase(
            submissions, activities, new FakeSectionRepository(), new FakeCourseRepository(), users, storage, new FakeNotificationRepository(), new FakeEmailSender());

        return (activities, users, submissions, storage, useCase);
    }

    private static User CreateStudentWithName() =>
        User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante, "Ana Gómez");

    [Fact]
    public void Execute_WithinFileLimit_StoresFilesAndSavesSubmission()
    {
        var (activities, users, submissions, storage, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", FutureDueDate, ActivityType.ConArchivo, maxFiles: 2);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);
        var files = new List<SubmittedFileInput> { new("tarea.pdf", [1, 2, 3]) };

        var result = useCase.Execute(new SubmitFilesCommand(activity.Id, student.Id, files));

        Assert.Single(result.Files);
        Assert.Single(storage.StoredFiles);
        Assert.Single(submissions.SavedSubmissions);
    }

    [Fact]
    public void Execute_ExceedingMaxFiles_Throws()
    {
        var (activities, users, _, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", FutureDueDate, ActivityType.ConArchivo, maxFiles: 1);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);
        var files = new List<SubmittedFileInput> { new("a.pdf", [1]), new("b.pdf", [2]) };

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(new SubmitFilesCommand(activity.Id, student.Id, files)));
    }

    [Fact]
    public void Execute_WithDescription_SetsTextContentOnResult()
    {
        var (activities, users, _, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", FutureDueDate, ActivityType.ConArchivo, maxFiles: 2);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);
        var files = new List<SubmittedFileInput> { new("tarea.pdf", [1, 2, 3]) };

        var result = useCase.Execute(new SubmitFilesCommand(activity.Id, student.Id, files, "<p>Notas de la entrega</p>"));

        Assert.Equal("<p>Notas de la entrega</p>", result.TextContent);
    }

    [Fact]
    public void Execute_WhenActivityIsTextOnly_Throws()
    {
        var (activities, users, _, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);
        var files = new List<SubmittedFileInput> { new("a.pdf", [1]) };

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitFilesCommand(activity.Id, student.Id, files)));
    }
}
