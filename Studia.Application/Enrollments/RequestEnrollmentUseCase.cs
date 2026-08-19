using Studia.Application.Courses;
using Studia.Application.Notifications;
using Studia.Application.Users;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;
using Studia.Domain.Users;

namespace Studia.Application.Enrollments;

public class RequestEnrollmentUseCase(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : IRequestEnrollmentUseCase
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

        NotifyProfesor(course, student);

        return EnrollmentResult.FromDomain(enrollment);
    }

    private void NotifyProfesor(Course course, User student)
    {
        var profesor = userRepository.GetById(course.ProfesorId);
        if (profesor is null)
            return;

        var notification = Notification.Create(
            profesor.Id,
            NotificationType.SolicitudInscripcion,
            "Nueva solicitud de inscripción",
            $"{student.Name} solicitó inscribirse en '{course.Name}'.",
            course.Id);

        emailSender.Send(profesor.Email.Value, notification.Title, notification.Message);
        notification.MarkEmailSent();

        notificationRepository.Save(notification);
    }
}
