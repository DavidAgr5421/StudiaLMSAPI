using Studia.Application.Cohorts;
using Studia.Application.Users;

namespace Studia.Application.Submissions;

public class GetSubmissionsByActivityUseCase(
    ISubmissionRepository submissionRepository,
    IUserRepository userRepository,
    ICohortRepository cohortRepository) : IGetSubmissionsByActivityUseCase
{
    public IReadOnlyCollection<SubmissionResult> Execute(GetSubmissionsByActivityQuery query) =>
        submissionRepository.GetByActivityId(query.ActivityId)
            .Select(SubmissionResult.FromDomain)
            .Select(WithStudentName)
            .Select(result => SubmissionGrouping.WithGroupName(result, cohortRepository))
            .ToList();

    private SubmissionResult WithStudentName(SubmissionResult result) =>
        result with { StudentName = userRepository.GetById(result.StudentId)?.Name };
}
