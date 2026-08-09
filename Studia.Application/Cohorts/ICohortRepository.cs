using Studia.Domain.Cohorts;

namespace Studia.Application.Cohorts;

public interface ICohortRepository
{
    void Save(Cohort cohort);

    Cohort? GetById(Guid id);

    IReadOnlyCollection<Cohort> GetByCourseId(Guid courseId);
}
