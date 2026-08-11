using System.Collections.Concurrent;
using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Infrastructure.Persistence;

public class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly ConcurrentDictionary<Guid, Enrollment> _enrollments = new();

    public void Save(Enrollment enrollment) => _enrollments[enrollment.Id] = enrollment;

    public Enrollment? GetById(Guid id) => _enrollments.GetValueOrDefault(id);

    public IReadOnlyCollection<Enrollment> GetByCourseId(Guid courseId) =>
        _enrollments.Values.Where(e => e.CourseId == courseId).ToList();

    public IReadOnlyCollection<Enrollment> GetByStudentId(Guid studentId) =>
        _enrollments.Values.Where(e => e.StudentId == studentId).ToList();

    public void DeleteByCourseId(Guid courseId)
    {
        foreach (var enrollment in GetByCourseId(courseId))
            _enrollments.TryRemove(enrollment.Id, out _);
    }
}
