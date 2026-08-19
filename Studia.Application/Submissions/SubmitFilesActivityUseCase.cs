using Studia.Application.Activities;
using Studia.Application.Cohorts;
using Studia.Application.Courses;
using Studia.Application.Notifications;
using Studia.Application.Sections;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Notifications;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Application.Submissions;

public class SubmitFilesActivityUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository,
    ICohortRepository cohortRepository,
    IUserRepository userRepository,
    IFileStorage fileStorage,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : ISubmitFilesActivityUseCase
{
    public SubmissionResult Execute(SubmitFilesCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        if (activity.Type != ActivityType.ConArchivo)
            throw new InvalidOperationException("Esta actividad es de solo texto, no admite archivos.");

        if (!activity.AcceptsSubmissionsAt(DateTime.UtcNow))
            throw new InvalidOperationException("Esta actividad ya no acepta entregas.");

        var student = userRepository.GetById(command.StudentId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.StudentId}'.");

        if (student.Role != Role.Estudiante)
            throw new InvalidOperationException($"El usuario '{student.Email}' no tiene rol Estudiante.");

        if (string.IsNullOrWhiteSpace(student.Name))
            throw new InvalidOperationException("Debe completar su nombre antes de poder entregar una actividad.");

        Guid? groupId = null;
        if (activity.Kind == ActivityKind.Grupal)
        {
            var section = sectionRepository.GetById(activity.SectionId)
                ?? throw new InvalidOperationException($"No existe una sección con id '{activity.SectionId}'.");
            groupId = SubmissionGrouping.ResolveGroupId(activity, section.CourseId, student.Id, cohortRepository);
        }

        var existingSubmissions = submissionRepository.GetByActivityId(activity.Id);
        var alreadySubmitted = groupId is not null
            ? existingSubmissions.Any(s => s.GroupId == groupId)
            : existingSubmissions.Any(s => s.StudentId == student.Id);

        if (alreadySubmitted)
        {
            throw new InvalidOperationException(
                groupId is not null ? "Tu grupo ya entregó esta actividad." : $"El estudiante '{student.Email}' ya entregó esta actividad.");
        }

        var files = command.Files
            .Select(file =>
            {
                var storageKey = fileStorage.Store(file.FileName, file.Content);
                return SubmittedFile.Create(file.FileName, storageKey, file.Content.Length);
            })
            .ToList();

        var submission = Submission.SubmitWithFiles(
            activity.Id, student.Id, files, activity.MaxFiles!.Value, activity.DueDateUtc, command.Description, groupId);

        submissionRepository.Save(submission);

        NotifyProfesor(activity, student);

        return SubmissionGrouping.WithGroupName(SubmissionResult.FromDomain(submission), cohortRepository);
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
