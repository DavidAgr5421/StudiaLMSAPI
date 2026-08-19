using Studia.Application.Activities;
using Studia.Application.Enrollments;
using Studia.Application.Sections;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;

namespace Studia.Application.Notifications;

public class NotifyNewActivityUseCase(
    INotificationRepository notificationRepository,
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    IEnrollmentRepository enrollmentRepository,
    IUserRepository userRepository,
    IEmailSender emailSender) : INotifyNewActivityUseCase
{
    public IReadOnlyCollection<NotificationResult> Execute(NotifyNewActivityCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        // Oculto: el profesor todavía está preparando el contenido, nadie se entera.
        if (activity.Status == ActivityStatus.Oculto)
            return [];

        var section = sectionRepository.GetById(activity.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{activity.SectionId}'.");

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
                NotificationType.NuevaActividad,
                "Nueva actividad publicada",
                $"Se publicó la actividad '{activity.Title}' con fecha límite {activity.DueDateUtc:yyyy-MM-dd HH:mm} UTC.",
                activity.Id);

            emailSender.Send(student.Email.Value, notification.Title, notification.Message);
            notification.MarkEmailSent();

            notificationRepository.Save(notification);
            results.Add(NotificationResult.FromDomain(notification));
        }

        return results;
    }
}
