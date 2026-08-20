using Studia.Application.Cohorts;
using Studia.Domain.Submissions;

namespace Studia.Application.Submissions;

// Una entrega Grupal le pertenece a la ficha entera, no solo a quien la subió --
// cualquier miembro del grupo puede verla/editarla.
public static class SubmissionOwnership
{
    public static bool BelongsTo(Submission submission, Guid studentId, ICohortRepository cohortRepository)
    {
        if (submission.StudentId == studentId)
            return true;

        if (submission.GroupId is null)
            return false;

        var cohort = cohortRepository.GetById(submission.GroupId.Value);
        return cohort is not null && cohort.StudentIds.Contains(studentId);
    }
}
