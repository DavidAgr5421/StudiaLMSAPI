namespace Studia.Application.Submissions;

public interface ISubmitTextActivityUseCase
{
    SubmissionResult Execute(SubmitTextCommand command);
}
