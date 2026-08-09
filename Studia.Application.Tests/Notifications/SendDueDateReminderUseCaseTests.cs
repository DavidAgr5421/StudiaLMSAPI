using Studia.Application.Notifications;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Enrollments;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Submissions;
using Studia.Application.Tests.Users;
using Studia.Domain.Activities;
using Studia.Domain.Enrollments;
using Studia.Domain.Sections;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Notifications;

public class SendDueDateReminderUseCaseTests
{
    private static readonly DateTime FutureDueDate = DateTime.UtcNow.AddDays(1);

    [Fact]
    public void Execute_OnlyNotifiesStudentsWhoHaveNotSubmitted()
    {
        var sections = new FakeSectionRepository();
        var courseId = Guid.NewGuid();
        var section = Section.Create(courseId, "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(section.Id, "Tarea", "Descripción", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);

        var submittedStudent = User.Register(Email.Create("entrego@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        var pendingStudent = User.Register(Email.Create("pendiente@sena.edu.co"), "hash", Role.Estudiante, "Luis");
        var users = new FakeUserRepository();
        users.Save(submittedStudent);
        users.Save(pendingStudent);

        var enrollments = new FakeEnrollmentRepository();
        enrollments.Save(Enrollment.Enroll(courseId, submittedStudent.Id));
        enrollments.Save(Enrollment.Enroll(courseId, pendingStudent.Id));

        var submissions = new FakeSubmissionRepository();
        submissions.Save(Submission.SubmitText(activity.Id, submittedStudent.Id, "Ya entregué", FutureDueDate));

        var notifications = new FakeNotificationRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new SendDueDateReminderUseCase(notifications, activities, sections, enrollments, submissions, users, emailSender);

        var results = useCase.Execute(new SendDueDateReminderCommand(activity.Id));

        var result = Assert.Single(results);
        Assert.Equal(pendingStudent.Id, result.RecipientUserId);
    }

    [Fact]
    public void Execute_WhenActivityDoesNotExist_Throws()
    {
        var useCase = new SendDueDateReminderUseCase(
            new FakeNotificationRepository(),
            new FakeActivityRepository(),
            new FakeSectionRepository(),
            new FakeEnrollmentRepository(),
            new FakeSubmissionRepository(),
            new FakeUserRepository(),
            new FakeEmailSender());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new SendDueDateReminderCommand(Guid.NewGuid())));
    }
}
