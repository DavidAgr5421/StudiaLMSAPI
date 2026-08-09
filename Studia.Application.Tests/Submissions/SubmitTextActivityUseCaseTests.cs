using Studia.Application.Submissions;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Users;
using Studia.Domain.Activities;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Submissions;

public class SubmitTextActivityUseCaseTests
{
    private static readonly DateTime FutureDueDate = DateTime.UtcNow.AddDays(1);

    private static (FakeActivityRepository Activities, FakeUserRepository Users, FakeSubmissionRepository Submissions, SubmitTextActivityUseCase UseCase) CreateSut()
    {
        var activities = new FakeActivityRepository();
        var users = new FakeUserRepository();
        var submissions = new FakeSubmissionRepository();
        var useCase = new SubmitTextActivityUseCase(submissions, activities, users);

        return (activities, users, submissions, useCase);
    }

    private static User CreateStudentWithName(string email = "estudiante@sena.edu.co", string name = "Ana Gómez") =>
        User.Register(Email.Create(email), "hashed-value", Role.Estudiante, name);

    [Fact]
    public void Execute_WithTextActivityAndNamedStudent_SavesSubmission()
    {
        var (activities, users, submissions, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);

        var result = useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Mi respuesta"));

        Assert.Equal(Studia.Domain.Submissions.SubmissionStatus.ATiempo, result.Status);
        Assert.Single(submissions.SavedSubmissions);
    }

    [Fact]
    public void Execute_WhenStudentHasNoName_Throws()
    {
        var (activities, users, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);
        var student = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante);
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Mi respuesta")));
    }

    [Fact]
    public void Execute_WhenActivityRequiresFiles_Throws()
    {
        var (activities, users, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Tarea", "Suba su documento", FutureDueDate, ActivityType.ConArchivo, 2);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Mi respuesta")));
    }

    [Fact]
    public void Execute_WhenAlreadySubmitted_Throws()
    {
        var (activities, users, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);
        var student = CreateStudentWithName();
        users.Save(student);
        useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Mi respuesta"));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Otra respuesta")));
    }

    [Fact]
    public void Execute_WhenTeacherTriesToSubmit_Throws()
    {
        var (activities, users, _, useCase) = CreateSut();
        var activity = Activity.Create(Guid.NewGuid(), "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);
        var teacher = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "Profe Ana");
        users.Save(teacher);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, teacher.Id, "Mi respuesta")));
    }
}
