namespace Studia.Application.Cohorts;

public interface IAssignStudentToCohortUseCase
{
    CohortResult Execute(AssignStudentToCohortCommand command);
}
