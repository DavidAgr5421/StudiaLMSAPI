namespace Studia.Application.Submissions;

public interface IGetSubmissionFileUseCase
{
    SubmissionFileContentResult Execute(GetSubmissionFileQuery query);
}
