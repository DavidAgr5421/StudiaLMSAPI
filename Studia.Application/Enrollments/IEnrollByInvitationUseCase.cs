namespace Studia.Application.Enrollments;

public interface IEnrollByInvitationUseCase
{
    EnrollmentResult Execute(EnrollByInvitationCommand command);
}
