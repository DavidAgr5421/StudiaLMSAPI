using Studia.Application.Activities;
using Studia.Application.Cohorts;

namespace Studia.Application.Submissions;

public class EditTextSubmissionUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    ICohortRepository cohortRepository)
    : IEditTextSubmissionUseCase
{
    public SubmissionResult Execute(EditTextSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        if (!SubmissionOwnership.BelongsTo(submission, command.StudentId, cohortRepository))
            throw new InvalidOperationException("No puede editar la entrega de otro estudiante.");

        var activity = activityRepository.GetById(submission.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{submission.ActivityId}'.");

        if (!activity.AcceptsSubmissionsAt(DateTime.UtcNow))
            throw new InvalidOperationException("Esta actividad ya no acepta entregas.");

        submission.EditText(command.TextContent);

        submissionRepository.Save(submission);

        return SubmissionGrouping.WithGroupName(SubmissionResult.FromDomain(submission), cohortRepository);
    }
}
