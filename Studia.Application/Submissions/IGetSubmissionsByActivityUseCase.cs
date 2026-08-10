namespace Studia.Application.Submissions;

public interface IGetSubmissionsByActivityUseCase
{
    IReadOnlyCollection<SubmissionResult> Execute(GetSubmissionsByActivityQuery query);
}
