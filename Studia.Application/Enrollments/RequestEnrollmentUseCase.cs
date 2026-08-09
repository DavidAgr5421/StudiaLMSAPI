using Studia.Application.Courses;
using Studia.Application.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Enrollments;

public class RequestEnrollmentUseCase(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository) : IRequestEnrollmentUseCase
{
    public EnrollmentResult Execute(RequestEnrollmentCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        if (course.EnrollmentMode != EnrollmentMode.ConAprobacion)
            throw new InvalidOperationException($"El curso '{course.Name}' no requiere aprobación para inscribirse.");

        if (course.Status != CourseStatus.Activo)
            throw new InvalidOperationException($"El curso '{course.Name}' no está activo.");

        var student = userRepository.GetById(command.StudentId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.StudentId}'.");

        if (student.Role != Role.Estudiante)
            throw new InvalidOperationException($"El usuario '{student.Email}' no tiene rol Estudiante.");

        var hasActiveEnrollment = enrollmentRepository.GetByCourseId(course.Id)
            .Any(e => e.StudentId == student.Id && e.Status != EnrollmentStatus.Rechazada);

        if (hasActiveEnrollment)
            throw new InvalidOperationException($"El estudiante '{student.Email}' ya tiene una inscripción activa o pendiente en este curso.");

        var enrollment = Enrollment.RequestEnrollment(course.Id, student.Id);

        enrollmentRepository.Save(enrollment);

        return EnrollmentResult.FromDomain(enrollment);
    }
}
