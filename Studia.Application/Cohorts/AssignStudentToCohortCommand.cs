namespace Studia.Application.Cohorts;

public record AssignStudentToCohortCommand(Guid CohortId, Guid StudentId);
