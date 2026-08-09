using Studia.Application.Activities;
using Studia.Application.Users;
using Studia.Domain.Activities;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Application.Submissions;

public class SubmitFilesActivityUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    IUserRepository userRepository,
    IFileStorage fileStorage) : ISubmitFilesActivityUseCase
{
    public SubmissionResult Execute(SubmitFilesCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        if (activity.Type != ActivityType.ConArchivo)
            throw new InvalidOperationException("Esta actividad es de solo texto, no admite archivos.");

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

        var files = command.Files
            .Select(file =>
            {
                var storageKey = fileStorage.Store(file.FileName, file.Content);
                return SubmittedFile.Create(file.FileName, storageKey, file.Content.Length);
            })
            .ToList();

        var submission = Submission.SubmitWithFiles(activity.Id, student.Id, files, activity.MaxFiles!.Value, activity.DueDateUtc);

        submissionRepository.Save(submission);

        return SubmissionResult.FromDomain(submission);
    }
}
