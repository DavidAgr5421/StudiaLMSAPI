using Studia.Application.Users;

namespace Studia.Application.Courses;

public class GetCourseByIdUseCase(ICourseRepository courseRepository, IUserRepository userRepository) : IGetCourseByIdUseCase
{
    public CourseResult? Execute(GetCourseByIdQuery query)
    {
        var course = courseRepository.GetById(query.CourseId);
        if (course is null) return null;

        var profesor = userRepository.GetById(course.ProfesorId);

        return CourseResult.FromDomain(course) with { ProfesorName = profesor?.Name };
    }
}
