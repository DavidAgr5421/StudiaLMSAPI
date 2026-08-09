namespace Studia.Application.Enrollments;

public interface IRequestEnrollmentUseCase
{
    EnrollmentResult Execute(RequestEnrollmentCommand command);
}
