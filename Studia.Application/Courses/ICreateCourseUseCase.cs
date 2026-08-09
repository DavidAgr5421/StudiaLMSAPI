namespace Studia.Application.Courses;

public interface ICreateCourseUseCase
{
    CourseResult Execute(CreateCourseCommand command);
}
