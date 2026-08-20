using Studia.Application.Cohorts;

namespace Studia.Application.Submissions;

// "Mi entrega": a diferencia de GetSubmissionsByActivityUseCase (vista del profesor, todas
// las entregas), acá el estudiante solo puede ver la propia (o la de su grupo, si la
// actividad es Grupal) -- el filtro es lo que evita que alguien vea la entrega de un
// compañero de otro grupo adivinando el activityId.
public class GetSubmissionForActivityUseCase(
    ISubmissionRepository submissionRepository,
    ICohortRepository cohortRepository) : IGetSubmissionForActivityUseCase
{
    public SubmissionResult? Execute(GetSubmissionForActivityQuery query)
    {
        var submission = submissionRepository.GetByActivityId(query.ActivityId)
            .FirstOrDefault(s => SubmissionOwnership.BelongsTo(s, query.StudentId, cohortRepository));

        return submission is null ? null : SubmissionGrouping.WithGroupName(SubmissionResult.FromDomain(submission), cohortRepository);
    }
}
