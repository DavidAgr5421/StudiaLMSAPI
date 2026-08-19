namespace Studia.Application.Submissions;

public interface IEditTextSubmissionUseCase
{
    SubmissionResult Execute(EditTextSubmissionCommand command);
}
