using Studia.Application.Courses;
using Studia.Application.Notifications;
using Studia.Application.Users;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Notifications;
using Studia.Domain.Users;

namespace Studia.Application.Cohorts;

// Versión en lote de AssignStudentToCohortUseCase: el profesor arma la lista buscando con
// GET /api/users/search y la manda de una sola vez. Igual que AddStudentsToCourseUseCase,
// si uno falla no tumba a los demás -- revisá "Outcomes" para ver cuál entró y cuál no.
public class AssignStudentsToCohortUseCase(
    ICohortRepository cohortRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IEmailSender emailSender) : IAssignStudentsToCohortUseCase
{
    public AssignStudentsToCohortResult Execute(AssignStudentsToCohortCommand command)
    {
        var cohort = cohortRepository.GetById(command.CohortId)
            ?? throw new InvalidOperationException($"No existe una ficha con id '{command.CohortId}'.");

        var studentIdsInOtherCohorts = cohortRepository.GetByCourseId(cohort.CourseId)
            .Where(c => c.Id != cohort.Id)
            .SelectMany(c => c.StudentIds)
            .ToHashSet();

        var course = courseRepository.GetById(cohort.CourseId);

        var assignedStudents = new List<User>();
        var outcomes = command.StudentIdentifiers
            .Select(identifier => TryAssignStudent(cohort, identifier, studentIdsInOtherCohorts, assignedStudents))
            .ToList();

        cohortRepository.Save(cohort);

        if (course is not null)
            foreach (var student in assignedStudents)
                NotifyStudent(course, cohort.Name, student);

        return new AssignStudentsToCohortResult(CohortResult.FromDomain(cohort), outcomes);
    }

    private AssignStudentToCohortOutcome TryAssignStudent(
        Cohort cohort, string identifier, HashSet<Guid> studentIdsInOtherCohorts, List<User> assignedStudents)
    {
        var student = ResolveStudent(identifier);

        if (student is null)
            return new AssignStudentToCohortOutcome(identifier, false, $"No se encontró un usuario con identificador '{identifier}'.");

        if (student.Role != Role.Estudiante)
            return new AssignStudentToCohortOutcome(identifier, false, $"El usuario '{student.Email}' no tiene rol Estudiante.");

        if (studentIdsInOtherCohorts.Contains(student.Id))
            return new AssignStudentToCohortOutcome(identifier, false, $"El estudiante '{student.Email}' ya pertenece a otra ficha de este curso.");

        try
        {
            cohort.AssignStudent(student.Id);
        }
        catch (InvalidOperationException ex)
        {
            return new AssignStudentToCohortOutcome(identifier, false, ex.Message);
        }

        assignedStudents.Add(student);
        return new AssignStudentToCohortOutcome(identifier, true, null);
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

    private void NotifyStudent(Course course, string cohortName, User student)
    {
        var notification = Notification.Create(
            student.Id,
            NotificationType.MovidoAFicha,
            "Te asignaron a una ficha",
            $"Te asignaron a la ficha '{cohortName}' en el curso '{course.Name}'.",
            course.Id);

        emailSender.Send(student.Email.Value, notification.Title, notification.Message);
        notification.MarkEmailSent();

        notificationRepository.Save(notification);
    }
}
