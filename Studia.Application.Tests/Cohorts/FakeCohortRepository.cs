using Studia.Application.Cohorts;
using Studia.Domain.Cohorts;

namespace Studia.Application.Tests.Cohorts;

public class FakeCohortRepository : ICohortRepository
{
    private readonly Dictionary<Guid, Cohort> _cohorts = new();

    public IReadOnlyCollection<Cohort> SavedCohorts => _cohorts.Values.ToList();

    public void Save(Cohort cohort) => _cohorts[cohort.Id] = cohort;

    public Cohort? GetById(Guid id) => _cohorts.GetValueOrDefault(id);

    public IReadOnlyCollection<Cohort> GetByCourseId(Guid courseId) =>
        _cohorts.Values.Where(c => c.CourseId == courseId).ToList();

    public IReadOnlyCollection<Cohort> Search(string query) =>
        _cohorts.Values
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public void DeleteByCourseId(Guid courseId)
    {
        foreach (var cohort in GetByCourseId(courseId))
            _cohorts.Remove(cohort.Id);
    }
}
