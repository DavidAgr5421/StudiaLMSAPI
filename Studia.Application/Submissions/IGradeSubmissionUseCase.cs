namespace Studia.Application.Submissions;

public interface IGradeSubmissionUseCase
{
    SubmissionResult Execute(GradeSubmissionCommand command);
}
