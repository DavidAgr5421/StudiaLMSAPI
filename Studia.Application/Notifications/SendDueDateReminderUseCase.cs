using Studia.Application.Activities;
using Studia.Application.Enrollments;
using Studia.Application.Sections;
using Studia.Application.Submissions;
using Studia.Application.Users;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;

namespace Studia.Application.Notifications;

public class SendDueDateReminderUseCase(
    INotificationRepository notificationRepository,
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    IEnrollmentRepository enrollmentRepository,
    ISubmissionRepository submissionRepository,
    IUserRepository userRepository,
    IEmailSender emailSender) : ISendDueDateReminderUseCase
{
    public IReadOnlyCollection<NotificationResult> Execute(SendDueDateReminderCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        var section = sectionRepository.GetById(activity.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{activity.SectionId}'.");

        var studentsWhoSubmitted = submissionRepository.GetByActivityId(activity.Id)
            .Select(s => s.StudentId)
            .ToHashSet();

        var pendingStudentIds = enrollmentRepository.GetByCourseId(section.CourseId)
            .Where(e => e.Status == EnrollmentStatus.Aprobada && !studentsWhoSubmitted.Contains(e.StudentId))
            .Select(e => e.StudentId);

        var results = new List<NotificationResult>();

        foreach (var studentId in pendingStudentIds)
        {
            var student = userRepository.GetById(studentId);
            if (student is null)
                continue;

            var notification = Notification.Create(
                student.Id,
                NotificationType.RecordatorioFechaLimite,
                "Fecha límite próxima",
                $"La actividad '{activity.Title}' vence el {activity.DueDateUtc:yyyy-MM-dd HH:mm} UTC y aún no la has entregado.",
                activity.Id);

            emailSender.Send(student.Email.Value, notification.Title, notification.Message);
            notification.MarkEmailSent();

            notificationRepository.Save(notification);
            results.Add(NotificationResult.FromDomain(notification));
        }

        return results;
    }
}
