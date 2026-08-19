using Studia.Application.Activities;
using Studia.Application.Courses;
using Studia.Application.Notifications;
using Studia.Application.Sections;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Notifications;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Application.Submissions;

public class SubmitTextActivityUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : ISubmitTextActivityUseCase
{
    public SubmissionResult Execute(SubmitTextCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        if (activity.Type != ActivityType.SoloTexto)
            throw new InvalidOperationException("Esta actividad requiere archivos adjuntos, no texto.");

        var student = userRepository.GetById(command.StudentId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.StudentId}'.");

        if (student.Role != Role.Estudiante)
            throw new InvalidOperationException($"El usuario '{student.Email}' no tiene rol Estudiante.");

        if (string.IsNullOrWhiteSpace(student.Name))
            throw new InvalidOperationException("Debe completar su nombre antes de poder entregar una actividad.");

        var alreadySubmitted = submissionRepository.GetByActivityId(activity.Id)
            .Any(s => s.StudentId == student.Id);

        if (alreadySubmitted)
            throw new InvalidOperationException($"El estudiante '{student.Email}' ya entregó esta actividad.");

        var submission = Submission.SubmitText(activity.Id, student.Id, command.TextContent, activity.DueDateUtc);

        submissionRepository.Save(submission);

        NotifyProfesor(activity, student);

        return SubmissionResult.FromDomain(submission);
    }

    private void NotifyProfesor(Activity activity, User student)
    {
        var section = sectionRepository.GetById(activity.SectionId);
        var course = section is null ? null : courseRepository.GetById(section.CourseId);
        var profesor = course is null ? null : userRepository.GetById(course.ProfesorId);
        if (profesor is null)
            return;

        var notification = Notification.Create(
            profesor.Id,
            NotificationType.EntregaActividad,
            "Nueva entrega recibida",
            $"{student.Name} entregó la actividad '{activity.Title}'.",
            activity.Id);

        emailSender.Send(profesor.Email.Value, notification.Title, notification.Message);
        notification.MarkEmailSent();

        notificationRepository.Save(notification);
    }
}
