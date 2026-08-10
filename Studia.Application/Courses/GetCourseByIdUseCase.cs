namespace Studia.Application.Courses;

public class GetCourseByIdUseCase(ICourseRepository courseRepository) : IGetCourseByIdUseCase
{
    public CourseResult? Execute(GetCourseByIdQuery query)
    {
        var course = courseRepository.GetById(query.CourseId);

        return course is null ? null : CourseResult.FromDomain(course);
    }
}
