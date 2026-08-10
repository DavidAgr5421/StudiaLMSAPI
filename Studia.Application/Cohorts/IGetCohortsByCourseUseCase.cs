namespace Studia.Application.Cohorts;

public interface IGetCohortsByCourseUseCase
{
    IReadOnlyCollection<CohortResult> Execute(GetCohortsByCourseQuery query);
}
