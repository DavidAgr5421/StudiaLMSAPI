using Studia.Domain.Enrollments;

namespace Studia.Application.Enrollments;

public interface IEnrollmentRepository
{
    void Save(Enrollment enrollment);

    Enrollment? GetById(Guid id);

    IReadOnlyCollection<Enrollment> GetByCourseId(Guid courseId);

    void DeleteByCourseId(Guid courseId);
}
