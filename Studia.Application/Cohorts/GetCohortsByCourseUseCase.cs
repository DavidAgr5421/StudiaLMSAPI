namespace Studia.Application.Cohorts;

public class GetCohortsByCourseUseCase(ICohortRepository cohortRepository) : IGetCohortsByCourseUseCase
{
    public IReadOnlyCollection<CohortResult> Execute(GetCohortsByCourseQuery query) =>
        cohortRepository.GetByCourseId(query.CourseId)
            .Select(CohortResult.FromDomain)
            .ToList();
}
