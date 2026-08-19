namespace Studia.Application.Submissions;

public interface IEditFilesSubmissionUseCase
{
    SubmissionResult Execute(EditFilesSubmissionCommand command);
}
