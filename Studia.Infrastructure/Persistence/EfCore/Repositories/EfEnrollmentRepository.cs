using Microsoft.EntityFrameworkCore;
using Studia.Application.Enrollments;
using Studia.Domain.Enrollments;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfEnrollmentRepository(StudiaDbContext dbContext) : IEnrollmentRepository
{
    public void Save(Enrollment enrollment)
    {
        if (dbContext.Enrollments.Any(e => e.Id == enrollment.Id))
            dbContext.Enrollments.Update(enrollment);
        else
            dbContext.Enrollments.Add(enrollment);

        dbContext.SaveChanges();
    }

    public Enrollment? GetById(Guid id) => dbContext.Enrollments.FirstOrDefault(e => e.Id == id);

    public IReadOnlyCollection<Enrollment> GetByCourseId(Guid courseId) =>
        dbContext.Enrollments.Where(e => e.CourseId == courseId).ToList();

    public IReadOnlyCollection<Enrollment> GetByStudentId(Guid studentId) =>
        dbContext.Enrollments.Where(e => e.StudentId == studentId).ToList();

    public void DeleteByCourseId(Guid courseId) => dbContext.Enrollments.Where(e => e.CourseId == courseId).ExecuteDelete();
}
