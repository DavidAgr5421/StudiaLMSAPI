namespace Studia.Application.Submissions;

public class GetSubmissionsByActivityUseCase(ISubmissionRepository submissionRepository) : IGetSubmissionsByActivityUseCase
{
    public IReadOnlyCollection<SubmissionResult> Execute(GetSubmissionsByActivityQuery query) =>
        submissionRepository.GetByActivityId(query.ActivityId)
            .Select(SubmissionResult.FromDomain)
            .ToList();
}
