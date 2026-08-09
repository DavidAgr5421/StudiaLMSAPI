using Studia.Application.Enrollments;
using Studia.Application.Sections;
using Studia.Application.Users;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;

namespace Studia.Application.Notifications;

public class NotifyNewSectionUseCase(
    INotificationRepository notificationRepository,
    ISectionRepository sectionRepository,
    IEnrollmentRepository enrollmentRepository,
    IUserRepository userRepository,
    IEmailSender emailSender) : INotifyNewSectionUseCase
{
    public IReadOnlyCollection<NotificationResult> Execute(NotifyNewSectionCommand command)
    {
        var section = sectionRepository.GetById(command.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{command.SectionId}'.");

        var enrolledStudentIds = enrollmentRepository.GetByCourseId(section.CourseId)
            .Where(e => e.Status == EnrollmentStatus.Aprobada)
            .Select(e => e.StudentId);

        var results = new List<NotificationResult>();

        foreach (var studentId in enrolledStudentIds)
        {
            var student = userRepository.GetById(studentId);
            if (student is null)
                continue;

            var notification = Notification.Create(
                student.Id,
                NotificationType.ContenidoAgregado,
                "Nuevo contenido en el curso",
                $"Se agregó la sección '{section.Title}' al curso.",
                section.Id);

            emailSender.Send(student.Email.Value, notification.Title, notification.Message);
            notification.MarkEmailSent();

            notificationRepository.Save(notification);
            results.Add(NotificationResult.FromDomain(notification));
        }

        return results;
    }
}
