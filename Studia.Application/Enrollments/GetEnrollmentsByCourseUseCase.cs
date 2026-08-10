namespace Studia.Application.Enrollments;

public class GetEnrollmentsByCourseUseCase(IEnrollmentRepository enrollmentRepository) : IGetEnrollmentsByCourseUseCase
{
    public IReadOnlyCollection<EnrollmentResult> Execute(GetEnrollmentsByCourseQuery query) =>
        enrollmentRepository.GetByCourseId(query.CourseId)
            .Select(EnrollmentResult.FromDomain)
            .ToList();
}
