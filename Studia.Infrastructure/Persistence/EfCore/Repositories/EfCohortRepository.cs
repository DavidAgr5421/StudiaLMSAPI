using Microsoft.EntityFrameworkCore;
using Studia.Application.Cohorts;
using Studia.Domain.Cohorts;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfCohortRepository(StudiaDbContext dbContext) : ICohortRepository
{
    public void Save(Cohort cohort)
    {
        if (dbContext.Cohorts.Any(c => c.Id == cohort.Id))
            dbContext.Cohorts.Update(cohort);
        else
            dbContext.Cohorts.Add(cohort);

        dbContext.SaveChanges();
    }

    public Cohort? GetById(Guid id) => dbContext.Cohorts.FirstOrDefault(c => c.Id == id);

    public IReadOnlyCollection<Cohort> GetByCourseId(Guid courseId) =>
        dbContext.Cohorts.Where(c => c.CourseId == courseId).ToList();

    public IReadOnlyCollection<Cohort> Search(string query) =>
        dbContext.Cohorts
            .AsEnumerable()
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
}
