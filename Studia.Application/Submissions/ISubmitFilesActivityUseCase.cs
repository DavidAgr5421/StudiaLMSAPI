namespace Studia.Application.Submissions;

public interface ISubmitFilesActivityUseCase
{
    SubmissionResult Execute(SubmitFilesCommand command);
}
