namespace Studia.Application.Enrollments;

public interface IGetEnrollmentsByStudentUseCase
{
    IReadOnlyCollection<EnrollmentResult> Execute(GetEnrollmentsByStudentQuery query);
}
