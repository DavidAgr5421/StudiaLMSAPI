using Studia.Application.Activities;

namespace Studia.Application.Submissions;

public class EditTextSubmissionUseCase(ISubmissionRepository submissionRepository, IActivityRepository activityRepository)
    : IEditTextSubmissionUseCase
{
    public SubmissionResult Execute(EditTextSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        if (submission.StudentId != command.StudentId)
            throw new InvalidOperationException("No puede editar la entrega de otro estudiante.");

        var activity = activityRepository.GetById(submission.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{submission.ActivityId}'.");

        submission.EditText(command.TextContent, activity.DueDateUtc);

        submissionRepository.Save(submission);

        return SubmissionResult.FromDomain(submission);
    }
}
