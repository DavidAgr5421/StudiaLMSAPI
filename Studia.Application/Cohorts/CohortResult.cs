using Studia.Domain.Cohorts;

namespace Studia.Application.Cohorts;

public record CohortResult(Guid Id, Guid CourseId, string Name, IReadOnlyCollection<Guid> StudentIds)
{
    public static CohortResult FromDomain(Cohort cohort) =>
        new(cohort.Id, cohort.CourseId, cohort.Name, cohort.StudentIds);
}
