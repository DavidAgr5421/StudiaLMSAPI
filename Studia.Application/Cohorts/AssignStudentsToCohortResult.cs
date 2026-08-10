namespace Studia.Application.Cohorts;

public record AssignStudentsToCohortResult(CohortResult Cohort, IReadOnlyCollection<AssignStudentToCohortOutcome> Outcomes);
