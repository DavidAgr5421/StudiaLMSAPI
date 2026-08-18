namespace Studia.Application.Courses;

public interface IUpdateCourseColorUseCase
{
    CourseResult Execute(UpdateCourseColorCommand command);
}
