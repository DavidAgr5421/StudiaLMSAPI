namespace Studia.Application.Cohorts;

public record AssignStudentsToCohortCommand(Guid CohortId, IReadOnlyCollection<string> StudentIdentifiers);
