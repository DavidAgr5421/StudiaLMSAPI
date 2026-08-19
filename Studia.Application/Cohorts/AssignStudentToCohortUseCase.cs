using Studia.Application.Courses;
using Studia.Application.Notifications;
using Studia.Application.Users;
using Studia.Domain.Notifications;
using Studia.Domain.Users;

namespace Studia.Application.Cohorts;

public class AssignStudentToCohortUseCase(
    ICohortRepository cohortRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : IAssignStudentToCohortUseCase
{
    public CohortResult Execute(AssignStudentToCohortCommand command)
    {
        var cohort = cohortRepository.GetById(command.CohortId)
            ?? throw new InvalidOperationException($"No existe una ficha con id '{command.CohortId}'.");

        var student = userRepository.GetById(command.StudentId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.StudentId}'.");

        if (student.Role != Role.Estudiante)
            throw new InvalidOperationException($"El usuario '{student.Email}' no tiene rol Estudiante.");

        var otherCohortsInCourse = cohortRepository.GetByCourseId(cohort.CourseId)
            .Where(c => c.Id != cohort.Id);

        if (otherCohortsInCourse.Any(c => c.StudentIds.Contains(student.Id)))
            throw new InvalidOperationException($"El estudiante '{student.Email}' ya pertenece a otra ficha de este curso.");

        cohort.AssignStudent(student.Id);

        cohortRepository.Save(cohort);

        NotifyStudent(cohort.CourseId, cohort.Name, student);

        return CohortResult.FromDomain(cohort);
    }

    private void NotifyStudent(Guid courseId, string cohortName, User student)
    {
        var course = courseRepository.GetById(courseId);
        if (course is null)
            return;

        var notification = Notification.Create(
            student.Id,
            NotificationType.MovidoAFicha,
            "Te asignaron a una ficha",
            $"Te asignaron a la ficha '{cohortName}' en el curso '{course.Name}'.",
            courseId);

        emailSender.Send(student.Email.Value, notification.Title, notification.Message);
        notification.MarkEmailSent();

        notificationRepository.Save(notification);
    }
}
