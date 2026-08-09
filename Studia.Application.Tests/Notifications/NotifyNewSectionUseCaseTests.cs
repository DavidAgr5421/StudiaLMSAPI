using Studia.Application.Notifications;
using Studia.Application.Tests.Enrollments;
using Studia.Application.Tests.Sections;
using Studia.Application.Tests.Users;
using Studia.Domain.Enrollments;
using Studia.Domain.Sections;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Notifications;

public class NotifyNewSectionUseCaseTests
{
    [Fact]
    public void Execute_NotifiesApprovedEnrolledStudents()
    {
        var sections = new FakeSectionRepository();
        var courseId = Guid.NewGuid();
        var section = Section.Create(courseId, "Semana 1", "");
        sections.Save(section);

        var student = User.Register(Email.Create("estudiante@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        var users = new FakeUserRepository();
        users.Save(student);

        var enrollments = new FakeEnrollmentRepository();
        enrollments.Save(Enrollment.Enroll(courseId, student.Id));

        var notifications = new FakeNotificationRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new NotifyNewSectionUseCase(notifications, sections, enrollments, users, emailSender);

        var results = useCase.Execute(new NotifyNewSectionCommand(section.Id));

        var result = Assert.Single(results);
        Assert.Equal(student.Id, result.RecipientUserId);
    }

    [Fact]
    public void Execute_WhenSectionDoesNotExist_Throws()
    {
        var useCase = new NotifyNewSectionUseCase(
            new FakeNotificationRepository(),
            new FakeSectionRepository(),
            new FakeEnrollmentRepository(),
            new FakeUserRepository(),
            new FakeEmailSender());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new NotifyNewSectionCommand(Guid.NewGuid())));
    }
}
