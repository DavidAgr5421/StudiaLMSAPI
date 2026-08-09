namespace Studia.Application.Enrollments;

public interface IApproveEnrollmentUseCase
{
    EnrollmentResult Execute(ApproveEnrollmentCommand command);
}
