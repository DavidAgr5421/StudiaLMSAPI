using Studia.Application.Courses;
using Studia.Domain.Courses;

namespace Studia.Application.Tests.Courses;

public class FakeCourseRepository : ICourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public IReadOnlyCollection<Course> SavedCourses => _courses.Values.ToList();

    public void Save(Course course) => _courses[course.Id] = course;

    public Course? GetById(Guid id) => _courses.GetValueOrDefault(id);

    public Course? GetByInvitationCode(string invitationCode) =>
        _courses.Values.FirstOrDefault(c => c.InvitationCode == invitationCode);

    public IReadOnlyCollection<Course> Search(string query) =>
        _courses.Values
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
}
