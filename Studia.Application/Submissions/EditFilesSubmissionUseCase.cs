using Studia.Application.Activities;
using Studia.Application.Cohorts;
using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

public class EditFilesSubmissionUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    ICohortRepository cohortRepository,
    IFileStorage fileStorage) : IEditFilesSubmissionUseCase
{
    public SubmissionResult Execute(EditFilesSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        if (!SubmissionOwnership.BelongsTo(submission, command.StudentId, cohortRepository))
            throw new InvalidOperationException("No puede editar la entrega de otro estudiante.");

        var activity = activityRepository.GetById(submission.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{submission.ActivityId}'.");

        if (!activity.AcceptsSubmissionsAt(DateTime.UtcNow))
            throw new InvalidOperationException("Esta actividad ya no acepta entregas.");

        var files = command.Files
            .Select(file =>
            {
                var storageKey = fileStorage.Store(file.FileName, file.Content);
                return SubmittedFile.Create(file.FileName, storageKey, file.Content.Length);
            })
            .ToList();

        submission.EditFiles(files, activity.MaxFiles!.Value, command.Description);

        submissionRepository.Save(submission);

        return SubmissionGrouping.WithGroupName(SubmissionResult.FromDomain(submission), cohortRepository);
    }
}
