namespace Studia.Application.Enrollments;

public interface IRejectEnrollmentUseCase
{
    EnrollmentResult Execute(RejectEnrollmentCommand command);
}
