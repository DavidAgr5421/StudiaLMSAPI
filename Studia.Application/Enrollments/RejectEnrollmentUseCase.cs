namespace Studia.Application.Enrollments;

public class RejectEnrollmentUseCase(IEnrollmentRepository enrollmentRepository) : IRejectEnrollmentUseCase
{
    public EnrollmentResult Execute(RejectEnrollmentCommand command)
    {
        var enrollment = enrollmentRepository.GetById(command.EnrollmentId)
            ?? throw new InvalidOperationException($"No existe una inscripción con id '{command.EnrollmentId}'.");

        enrollment.Reject();

        enrollmentRepository.Save(enrollment);

        return EnrollmentResult.FromDomain(enrollment);
    }
}
