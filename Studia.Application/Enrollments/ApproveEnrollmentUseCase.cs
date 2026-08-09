namespace Studia.Application.Enrollments;

public class ApproveEnrollmentUseCase(IEnrollmentRepository enrollmentRepository) : IApproveEnrollmentUseCase
{
    public EnrollmentResult Execute(ApproveEnrollmentCommand command)
    {
        var enrollment = enrollmentRepository.GetById(command.EnrollmentId)
            ?? throw new InvalidOperationException($"No existe una inscripción con id '{command.EnrollmentId}'.");

        enrollment.Approve();

        enrollmentRepository.Save(enrollment);

        return EnrollmentResult.FromDomain(enrollment);
    }
}
