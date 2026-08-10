namespace Studia.Application.Enrollments;

public interface IGetEnrollmentsByCourseUseCase
{
    IReadOnlyCollection<EnrollmentResult> Execute(GetEnrollmentsByCourseQuery query);
}
