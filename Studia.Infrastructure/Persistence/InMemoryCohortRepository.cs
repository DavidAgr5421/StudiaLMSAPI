using System.Collections.Concurrent;
using Studia.Application.Cohorts;
using Studia.Domain.Cohorts;

namespace Studia.Infrastructure.Persistence;

public class InMemoryCohortRepository : ICohortRepository
{
    private readonly ConcurrentDictionary<Guid, Cohort> _cohorts = new();

    public void Save(Cohort cohort) => _cohorts[cohort.Id] = cohort;

    public Cohort? GetById(Guid id) => _cohorts.GetValueOrDefault(id);

    public IReadOnlyCollection<Cohort> GetByCourseId(Guid courseId) =>
        _cohorts.Values.Where(c => c.CourseId == courseId).ToList();

    public IReadOnlyCollection<Cohort> Search(string query) =>
        _cohorts.Values
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
}
