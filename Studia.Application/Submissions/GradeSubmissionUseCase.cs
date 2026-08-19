using Studia.Application.Activities;
using Studia.Application.Notifications;
using Studia.Application.Users;
using Studia.Domain.Notifications;

namespace Studia.Application.Submissions;

public class GradeSubmissionUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : IGradeSubmissionUseCase
{
    public SubmissionResult Execute(GradeSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        submission.Grade(command.Score, command.Feedback);

        submissionRepository.Save(submission);

        NotifyStudent(submission.ActivityId, submission.StudentId, command.Score);

        return SubmissionResult.FromDomain(submission);
    }

    private void NotifyStudent(Guid activityId, Guid studentId, int score)
    {
        var student = userRepository.GetById(studentId);
        var activity = activityRepository.GetById(activityId);
        if (student is null || activity is null)
            return;

        var notification = Notification.Create(
            student.Id,
            NotificationType.Calificado,
            "Tu entrega fue calificada",
            $"Tu entrega de '{activity.Title}' fue calificada con {score}/5.",
            activity.Id);

        emailSender.Send(student.Email.Value, notification.Title, notification.Message);
        notification.MarkEmailSent();

        notificationRepository.Save(notification);
    }
}
