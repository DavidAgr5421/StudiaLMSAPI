namespace Studia.Application.Cohorts;

public interface IAssignStudentsToCohortUseCase
{
    AssignStudentsToCohortResult Execute(AssignStudentsToCohortCommand command);
}
