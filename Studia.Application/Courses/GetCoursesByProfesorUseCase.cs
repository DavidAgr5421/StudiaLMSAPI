namespace Studia.Application.Courses;

public class GetCoursesByProfesorUseCase(ICourseRepository courseRepository) : IGetCoursesByProfesorUseCase
{
    public IReadOnlyCollection<CourseResult> Execute(GetCoursesByProfesorQuery query) =>
        courseRepository.GetByProfesorId(query.ProfesorId)
            .Select(CourseResult.FromDomain)
            .ToList();
}
