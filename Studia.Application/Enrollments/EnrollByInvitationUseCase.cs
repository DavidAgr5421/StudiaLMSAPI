using Studia.Application.Courses;
using Studia.Application.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Enrollments;

public class EnrollByInvitationUseCase(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository) : IEnrollByInvitationUseCase
{
    public EnrollmentResult Execute(EnrollByInvitationCommand command)
    {
        var course = courseRepository.GetByInvitationCode(command.InvitationCode)
            ?? throw new InvalidOperationException("El código de invitación no es válido.");

        if (course.Status != CourseStatus.Activo)
            throw new InvalidOperationException($"El curso '{course.Name}' no está activo.");

        var student = userRepository.GetById(command.StudentId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.StudentId}'.");

        if (student.Role != Role.Estudiante)
            throw new InvalidOperationException($"El usuario '{student.Email}' no tiene rol Estudiante.");

        var hasActiveEnrollment = enrollmentRepository.GetByCourseId(course.Id)
            .Any(e => e.StudentId == student.Id && e.Status != EnrollmentStatus.Rechazada);

        if (hasActiveEnrollment)
            throw new InvalidOperationException($"El estudiante '{student.Email}' ya está inscrito en este curso.");

        var enrollment = Enrollment.Enroll(course.Id, student.Id);

        enrollmentRepository.Save(enrollment);

        return EnrollmentResult.FromDomain(enrollment);
    }
}
