namespace Studia.Application.Cohorts;

public interface ICreateCohortUseCase
{
    CohortResult Execute(CreateCohortCommand command);
}
