using Studia.Application.Cohorts;

namespace Studia.Application.Courses;

public class SearchCoursesUseCase(ICourseRepository courseRepository, ICohortRepository cohortRepository) : ISearchCoursesUseCase
{
    public IReadOnlyCollection<CourseResult> Execute(SearchCoursesQuery query)
    {
        var matchedByName = courseRepository.Search(query.Query);

        var matchedByCohort = cohortRepository.Search(query.Query)
            .Select(cohort => courseRepository.GetById(cohort.CourseId))
            .Where(course => course is not null)
            .Select(course => course!);

        return matchedByName
            .Concat(matchedByCohort)
            .DistinctBy(course => course.Id)
            .Select(CourseResult.FromDomain)
            .ToList();
    }
}
