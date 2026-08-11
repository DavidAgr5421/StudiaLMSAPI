namespace Studia.Application.Submissions;

// "Mi entrega": a diferencia de GetSubmissionsByActivityUseCase (vista del profesor, todas
// las entregas), acá el estudiante solo puede ver la propia -- el filtro por StudentId no es
// opcional, es lo que evita que alguien vea la entrega de un compañero adivinando el activityId.
public class GetSubmissionForActivityUseCase(ISubmissionRepository submissionRepository) : IGetSubmissionForActivityUseCase
{
    public SubmissionResult? Execute(GetSubmissionForActivityQuery query)
    {
        var submission = submissionRepository.GetByActivityId(query.ActivityId)
            .FirstOrDefault(s => s.StudentId == query.StudentId);

        return submission is null ? null : SubmissionResult.FromDomain(submission);
    }
}
