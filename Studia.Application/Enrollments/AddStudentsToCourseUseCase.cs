using Studia.Application.Courses;
using Studia.Application.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Users;

namespace Studia.Application.Enrollments;

// RF11: el profesor añade estudiantes directamente, sin importar el modo de inscripción
// del curso (Abierta/ConAprobacion/PorInvitacion) -- eso solo aplica al auto-servicio.
public class AddStudentsToCourseUseCase(
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    IEnrollmentRepository enrollmentRepository) : IAddStudentsToCourseUseCase
{
    public AddStudentsToCourseResult Execute(AddStudentsToCourseCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        if (course.Status != CourseStatus.Activo)
            throw new InvalidOperationException($"El curso '{course.Name}' no está activo.");

        var outcomes = command.StudentIdentifiers
            .Select(identifier => TryAddStudent(course, identifier))
            .ToList();

        return new AddStudentsToCourseResult(outcomes);
    }

    private AddStudentToCourseOutcome TryAddStudent(Course course, string identifier)
    {
        var student = ResolveStudent(identifier);

        if (student is null)
            return new AddStudentToCourseOutcome(identifier, false, $"No se encontró un usuario con identificador '{identifier}'.", null);

        if (student.Role != Role.Estudiante)
            return new AddStudentToCourseOutcome(identifier, false, $"El usuario '{student.Email}' no tiene rol Estudiante.", null);

        var hasActiveEnrollment = enrollmentRepository.GetByCourseId(course.Id)
            .Any(e => e.StudentId == student.Id && e.Status != EnrollmentStatus.Rechazada);

        if (hasActiveEnrollment)
            return new AddStudentToCourseOutcome(identifier, false, $"El estudiante '{student.Email}' ya está inscrito en este curso.", null);

        var enrollment = Enrollment.Enroll(course.Id, student.Id);
        enrollmentRepository.Save(enrollment);

        return new AddStudentToCourseOutcome(identifier, true, null, EnrollmentResult.FromDomain(enrollment));
    }

    private User? ResolveStudent(string identifier)
    {
        if (Guid.TryParse(identifier, out var id))
            return userRepository.GetById(id);

        try
        {
            return userRepository.GetByEmail(Email.Create(identifier));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
