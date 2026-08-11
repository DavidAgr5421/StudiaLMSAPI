using Microsoft.EntityFrameworkCore;
using Studia.Application.Courses;
using Studia.Domain.Courses;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfCourseRepository(StudiaDbContext dbContext) : ICourseRepository
{
    public void Save(Course course)
    {
        if (dbContext.Courses.Any(c => c.Id == course.Id))
            dbContext.Courses.Update(course);
        else
            dbContext.Courses.Add(course);

        dbContext.SaveChanges();
    }

    public Course? GetById(Guid id) => dbContext.Courses.FirstOrDefault(c => c.Id == id);

    public Course? GetByInvitationCode(string invitationCode) =>
        dbContext.Courses.FirstOrDefault(c => c.InvitationCode == invitationCode);

    public IReadOnlyCollection<Course> Search(string query) =>
        dbContext.Courses
            .AsEnumerable()
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public IReadOnlyCollection<Course> GetByProfesorId(Guid profesorId) =>
        dbContext.Courses.Where(c => c.ProfesorId == profesorId).ToList();

    public void Delete(Guid id) => dbContext.Courses.Where(c => c.Id == id).ExecuteDelete();
}
