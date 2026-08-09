using Studia.Application.Notifications;
using Studia.Application.Tests.Activities;
using Studia.Application.Tests.Enrollments;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Users;
using Studia.Domain.Activities;
using Studia.Domain.Enrollments;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Notifications;

public class NotifyNewActivityUseCaseTests
{
    private static readonly DateTime FutureDueDate = DateTime.UtcNow.AddDays(1);

    [Fact]
    public void Execute_NotifiesOnlyApprovedEnrolledStudents()
    {
        var sections = new FakeSectionRepository();
        var section = Section.Create(Guid.NewGuid(), "Semana 1", "");
        sections.Save(section);

        var activities = new FakeActivityRepository();
        var activity = Activity.Create(section.Id, "Tarea", "Descripción", FutureDueDate, ActivityType.SoloTexto, null);
        activities.Save(activity);

        var enrolledStudent = User.Register(Email.Create("estudiante1@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        var pendingStudent = User.Register(Email.Create("estudiante2@sena.edu.co"), "hash", Role.Estudiante, "Luis");
        var users = new FakeUserRepository();
        users.Save(enrolledStudent);
        users.Save(pendingStudent);

        var enrollments = new FakeEnrollmentRepository();
        enrollments.Save(Enrollment.Enroll(section.CourseId, enrolledStudent.Id));
        enrollments.Save(Enrollment.RequestEnrollment(section.CourseId, pendingStudent.Id));

        var notifications = new FakeNotificationRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new NotifyNewActivityUseCase(notifications, activities, sections, enrollments, users, emailSender);

        var results = useCase.Execute(new NotifyNewActivityCommand(activity.Id));

        var result = Assert.Single(results);
        Assert.Equal(enrolledStudent.Id, result.RecipientUserId);
        Assert.True(result.EmailSent);
        Assert.Single(emailSender.SentEmails);
        Assert.Single(notifications.SavedNotifications);
    }

    [Fact]
    public void Execute_WhenActivityDoesNotExist_Throws()
    {
        var useCase = new NotifyNewActivityUseCase(
            new FakeNotificationRepository(),
            new FakeActivityRepository(),
            new FakeSectionRepository(),
            new FakeEnrollmentRepository(),
            new FakeUserRepository(),
            new FakeEmailSender());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new NotifyNewActivityCommand(Guid.NewGuid())));
    }
}
