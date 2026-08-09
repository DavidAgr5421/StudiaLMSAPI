using Studia.Domain.Courses;

namespace Studia.Application.Courses;

public interface ICourseRepository
{
    void Save(Course course);

    Course? GetById(Guid id);

    Course? GetByInvitationCode(string invitationCode);

    IReadOnlyCollection<Course> Search(string query);
}
