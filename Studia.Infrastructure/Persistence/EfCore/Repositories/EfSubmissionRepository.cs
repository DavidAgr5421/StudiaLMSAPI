using Microsoft.EntityFrameworkCore;
using Studia.Application.Submissions;
using Studia.Domain.Submissions;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfSubmissionRepository(StudiaDbContext dbContext) : ISubmissionRepository
{
    public void Save(Submission submission)
    {
        if (dbContext.Submissions.Any(s => s.Id == submission.Id))
            dbContext.Submissions.Update(submission);
        else
            dbContext.Submissions.Add(submission);

        dbContext.SaveChanges();
    }

    public Submission? GetById(Guid id) => dbContext.Submissions.FirstOrDefault(s => s.Id == id);

    public IReadOnlyCollection<Submission> GetByActivityId(Guid activityId) =>
        dbContext.Submissions.Where(s => s.ActivityId == activityId).ToList();

    public void DeleteByActivityId(Guid activityId) => dbContext.Submissions.Where(s => s.ActivityId == activityId).ExecuteDelete();
}
