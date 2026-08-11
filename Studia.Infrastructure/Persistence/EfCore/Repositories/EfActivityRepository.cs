using Microsoft.EntityFrameworkCore;
using Studia.Application.Activities;
using Studia.Domain.Activities;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfActivityRepository(StudiaDbContext dbContext) : IActivityRepository
{
    public void Save(Activity activity)
    {
        if (dbContext.Activities.Any(a => a.Id == activity.Id))
            dbContext.Activities.Update(activity);
        else
            dbContext.Activities.Add(activity);

        dbContext.SaveChanges();
    }

    public Activity? GetById(Guid id) => dbContext.Activities.FirstOrDefault(a => a.Id == id);

    public IReadOnlyCollection<Activity> GetBySectionId(Guid sectionId) =>
        dbContext.Activities.Where(a => a.SectionId == sectionId).ToList();

    public void DeleteBySectionId(Guid sectionId) => dbContext.Activities.Where(a => a.SectionId == sectionId).ExecuteDelete();
}
