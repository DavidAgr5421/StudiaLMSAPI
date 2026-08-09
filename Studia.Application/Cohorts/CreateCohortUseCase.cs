using Studia.Application.Courses;
using Studia.Domain.Cohorts;

namespace Studia.Application.Cohorts;

public class CreateCohortUseCase(ICohortRepository cohortRepository, ICourseRepository courseRepository)
    : ICreateCohortUseCase
{
    public CohortResult Execute(CreateCohortCommand command)
    {
        if (courseRepository.GetById(command.CourseId) is null)
            throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        var cohort = Cohort.Create(command.CourseId, command.Name);

        cohortRepository.Save(cohort);

        return CohortResult.FromDomain(cohort);
    }
}
