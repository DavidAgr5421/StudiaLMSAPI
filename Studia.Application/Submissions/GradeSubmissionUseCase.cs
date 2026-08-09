namespace Studia.Application.Submissions;

public class GradeSubmissionUseCase(ISubmissionRepository submissionRepository) : IGradeSubmissionUseCase
{
    public SubmissionResult Execute(GradeSubmissionCommand command)
    {
        var submission = submissionRepository.GetById(command.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{command.SubmissionId}'.");

        submission.Grade(command.Score, command.Feedback);

        submissionRepository.Save(submission);

        return SubmissionResult.FromDomain(submission);
    }
}
