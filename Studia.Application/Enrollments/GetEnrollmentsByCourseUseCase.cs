using Studia.Application.Users;

namespace Studia.Application.Enrollments;

public class GetEnrollmentsByCourseUseCase(
    IEnrollmentRepository enrollmentRepository,
    IUserRepository userRepository) : IGetEnrollmentsByCourseUseCase
{
    public IReadOnlyCollection<EnrollmentResult> Execute(GetEnrollmentsByCourseQuery query) =>
        enrollmentRepository.GetByCourseId(query.CourseId)
            .Select(EnrollmentResult.FromDomain)
            .Select(WithStudentInfo)
            .ToList();

    private EnrollmentResult WithStudentInfo(EnrollmentResult result)
    {
        var student = userRepository.GetById(result.StudentId);

        return result with { StudentName = student?.Name, StudentEmail = student?.Email.Value };
    }
}
