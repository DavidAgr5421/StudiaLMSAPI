using Studia.Application.Activities;
using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

public class EditFilesSubmissionUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    IFileStorage fileStorage) : IEditFilesSubmissionUseCase
{
    public SubmissionResult Execute(EditFilesSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        if (submission.StudentId != command.StudentId)
            throw new InvalidOperationException("No puede editar la entrega de otro estudiante.");

        var activity = activityRepository.GetById(submission.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{submission.ActivityId}'.");

        var files = command.Files
            .Select(file =>
            {
                var storageKey = fileStorage.Store(file.FileName, file.Content);
                return SubmittedFile.Create(file.FileName, storageKey, file.Content.Length);
            })
            .ToList();

        submission.EditFiles(files, activity.MaxFiles!.Value, activity.DueDateUtc, command.Description);

        submissionRepository.Save(submission);

        return SubmissionResult.FromDomain(submission);
    }
}
