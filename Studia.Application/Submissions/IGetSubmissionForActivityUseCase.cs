namespace Studia.Application.Submissions;

public interface IGetSubmissionForActivityUseCase
{
    SubmissionResult? Execute(GetSubmissionForActivityQuery query);
}
