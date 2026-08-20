using Studia.Application.Cohorts;
using Studia.Domain.Activities;

namespace Studia.Application.Submissions;

// Punto único donde se resuelve a qué ficha/grupo pertenece la entrega de un estudiante
// para una actividad Grupal. Individual/Foro/Evaluación no usan grupos: siempre null.
public static class SubmissionGrouping
{
    public static Guid? ResolveGroupId(Activity activity, Guid courseId, Guid studentId, ICohortRepository cohortRepository)
    {
        if (activity.Kind != ActivityKind.Grupal)
            return null;

        var cohort = cohortRepository.GetByCourseId(courseId)
            .FirstOrDefault(c => activity.CohortIds.Contains(c.Id) && c.StudentIds.Contains(studentId));

        if (cohort is null)
            throw new InvalidOperationException("No perteneces a ninguno de los grupos asignados a esta actividad.");

        return cohort.Id;
    }

    // FromDomain(Submission) no puede traer el nombre de la ficha -- vive en otro
    // aggregate. Cualquier caso de uso que devuelva una entrega Grupal pasa por acá.
    public static SubmissionResult WithGroupName(SubmissionResult result, ICohortRepository cohortRepository) =>
        result.GroupId is null ? result : result with { GroupName = cohortRepository.GetById(result.GroupId.Value)?.Name };
}
