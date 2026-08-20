using Studia.Application.Submissions;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Cohorts;
using Studia.Application.Tests.Courses;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Users;
using Studia.Domain.Activities;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Sections;
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
        var useCase = new SubmitTextActivityUseCase(
            submissions, activities, new FakeSectionRepository(), new FakeCourseRepository(), new FakeCohortRepository(), users, new FakeNotificationRepository(), new FakeEmailSender());

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
    public void Execute_NotifiesTheCourseProfesor()
    {
        var profesor = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "Profe Ana");
        var users = new FakeUserRepository();
        users.Save(profesor);
        var student = CreateStudentWithName();
        users.Save(student);

        var courses = new FakeCourseRepository();
        var course = Course.Create("English B1", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(section.Id, "Ensayo", "Escriba 200 palabras", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);

        var notifications = new FakeNotificationRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new SubmitTextActivityUseCase(
            new FakeSubmissionRepository(), activities, sections, courses, new FakeCohortRepository(), users, notifications, emailSender);

        useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Mi respuesta"));

        var notification = Assert.Single(notifications.SavedNotifications);
        Assert.Equal(profesor.Id, notification.RecipientUserId);
        Assert.Single(emailSender.SentEmails);
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

    [Fact]
    public void Execute_GrupalActivity_SetsGroupIdFromStudentsCohort()
    {
        var profesor = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "Profe Ana");
        var users = new FakeUserRepository();
        users.Save(profesor);
        var student = CreateStudentWithName();
        users.Save(student);

        var courses = new FakeCourseRepository();
        var course = Course.Create("English B1", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(course.Id, "Grupo A");
        cohort.AssignStudent(student.Id);
        cohorts.Save(cohort);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(
            section.Id, "Proyecto", "En grupo", FutureDueDate, ActivityType.SoloTexto, null,
            cohortIds: [cohort.Id], kind: ActivityKind.Grupal);
        activities.Save(activity);

        var submissions = new FakeSubmissionRepository();
        var useCase = new SubmitTextActivityUseCase(
            submissions, activities, sections, courses, cohorts, users, new FakeNotificationRepository(), new FakeEmailSender());

        var result = useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Respuesta del grupo"));

        Assert.Equal(cohort.Id, result.GroupId);
    }

    [Fact]
    public void Execute_GrupalActivity_WhenStudentNotInAnyCohort_Throws()
    {
        var profesor = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "Profe Ana");
        var users = new FakeUserRepository();
        users.Save(profesor);
        var student = CreateStudentWithName();
        users.Save(student);

        var courses = new FakeCourseRepository();
        var course = Course.Create("English B1", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(course.Id, "Grupo A");
        cohorts.Save(cohort);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(
            section.Id, "Proyecto", "En grupo", FutureDueDate, ActivityType.SoloTexto, null,
            cohortIds: [cohort.Id], kind: ActivityKind.Grupal);
        activities.Save(activity);

        var useCase = new SubmitTextActivityUseCase(
            new FakeSubmissionRepository(), activities, sections, courses, cohorts, users, new FakeNotificationRepository(), new FakeEmailSender());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, student.Id, "Respuesta del grupo")));
    }

    [Fact]
    public void Execute_GrupalActivity_WhenGroupAlreadySubmitted_Throws()
    {
        var profesor = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "Profe Ana");
        var users = new FakeUserRepository();
        users.Save(profesor);
        var studentA = CreateStudentWithName("a@sena.edu.co", "Ana");
        var studentB = CreateStudentWithName("b@sena.edu.co", "Beto");
        users.Save(studentA);
        users.Save(studentB);

        var courses = new FakeCourseRepository();
        var course = Course.Create("English B1", EnrollmentMode.Abierta, profesor.Id);
        courses.Save(course);

        var sections = new FakeSectionRepository();
        var section = Section.Create(course.Id, "Semana 1", "");
        sections.Save(section);

        var cohorts = new FakeCohortRepository();
        var cohort = Cohort.Create(course.Id, "Grupo A");
        cohort.AssignStudent(studentA.Id);
        cohort.AssignStudent(studentB.Id);
        cohorts.Save(cohort);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(
            section.Id, "Proyecto", "En grupo", FutureDueDate, ActivityType.SoloTexto, null,
            cohortIds: [cohort.Id], kind: ActivityKind.Grupal);
        activities.Save(activity);

        var submissions = new FakeSubmissionRepository();
        var useCase = new SubmitTextActivityUseCase(
            submissions, activities, sections, courses, cohorts, users, new FakeNotificationRepository(), new FakeEmailSender());
        useCase.Execute(new SubmitTextCommand(activity.Id, studentA.Id, "Respuesta del grupo"));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SubmitTextCommand(activity.Id, studentB.Id, "Otra respuesta")));
    }
}
