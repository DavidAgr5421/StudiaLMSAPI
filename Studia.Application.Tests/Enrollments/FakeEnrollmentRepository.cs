using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Application.Tests.Enrollments;

public class FakeEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<Guid, Enrollment> _enrollments = new();

    public IReadOnlyCollection<Enrollment> SavedEnrollments => _enrollments.Values.ToList();

    public void Save(Enrollment enrollment) => _enrollments[enrollment.Id] = enrollment;

    public Enrollment? GetById(Guid id) => _enrollments.GetValueOrDefault(id);

    public IReadOnlyCollection<Enrollment> GetByCourseId(Guid courseId) =>
        _enrollments.Values.Where(e => e.CourseId == courseId).ToList();
}
