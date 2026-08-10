using System.Collections.Concurrent;
using Studia.Application.Courses;
using Studia.Domain.Courses;

namespace Studia.Infrastructure.Persistence;

public class InMemoryCourseRepository : ICourseRepository
{
    private readonly ConcurrentDictionary<Guid, Course> _courses = new();

    public void Save(Course course) => _courses[course.Id] = course;

    public Course? GetById(Guid id) => _courses.GetValueOrDefault(id);

    public Course? GetByInvitationCode(string invitationCode) =>
        _courses.Values.FirstOrDefault(c => c.InvitationCode == invitationCode);

    public IReadOnlyCollection<Course> Search(string query) =>
        _courses.Values
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public IReadOnlyCollection<Course> GetByProfesorId(Guid profesorId) =>
        _courses.Values.Where(c => c.ProfesorId == profesorId).ToList();
}
