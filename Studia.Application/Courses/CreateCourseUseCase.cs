using Studia.Domain.Courses;

namespace Studia.Application.Courses;

public class CreateCourseUseCase(ICourseRepository courseRepository) : ICreateCourseUseCase
{
    public CourseResult Execute(CreateCourseCommand command)
    {
        var course = Course.Create(command.Name, command.EnrollmentMode);

        courseRepository.Save(course);

        return CourseResult.FromDomain(course);
    }
}
