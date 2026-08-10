using Microsoft.EntityFrameworkCore;
using Studia.Application.Sections;
using Studia.Domain.Sections;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfSectionRepository(StudiaDbContext dbContext) : ISectionRepository
{
    public void Save(Section section)
    {
        if (dbContext.Sections.Any(s => s.Id == section.Id))
            dbContext.Sections.Update(section);
        else
            dbContext.Sections.Add(section);

        dbContext.SaveChanges();
    }

    public Section? GetById(Guid id) => dbContext.Sections.FirstOrDefault(s => s.Id == id);

    public IReadOnlyCollection<Section> GetByCourseId(Guid courseId) =>
        dbContext.Sections.Where(s => s.CourseId == courseId).ToList();
}
