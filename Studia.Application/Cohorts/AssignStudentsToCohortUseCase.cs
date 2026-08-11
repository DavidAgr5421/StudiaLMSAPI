using Studia.Application.Users;
using Studia.Domain.Cohorts;
using Studia.Domain.Users;

namespace Studia.Application.Cohorts;

// Versión en lote de AssignStudentToCohortUseCase: el profesor arma la lista buscando con
// GET /api/users/search y la manda de una sola vez. Igual que AddStudentsToCourseUseCase,
// si uno falla no tumba a los demás -- revisá "Outcomes" para ver cuál entró y cuál no.
public class AssignStudentsToCohortUseCase(ICohortRepository cohortRepository, IUserRepository userRepository)
    : IAssignStudentsToCohortUseCase
{
    public AssignStudentsToCohortResult Execute(AssignStudentsToCohortCommand command)
    {
        var cohort = cohortRepository.GetById(command.CohortId)
            ?? throw new InvalidOperationException($"No existe una ficha con id '{command.CohortId}'.");

        var studentIdsInOtherCohorts = cohortRepository.GetByCourseId(cohort.CourseId)
            .Where(c => c.Id != cohort.Id)
            .SelectMany(c => c.StudentIds)
            .ToHashSet();

        var outcomes = command.StudentIdentifiers
            .Select(identifier => TryAssignStudent(cohort, identifier, studentIdsInOtherCohorts))
            .ToList();

        cohortRepository.Save(cohort);

        return new AssignStudentsToCohortResult(CohortResult.FromDomain(cohort), outcomes);
    }

    private AssignStudentToCohortOutcome TryAssignStudent(Cohort cohort, string identifier, HashSet<Guid> studentIdsInOtherCohorts)
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
}
