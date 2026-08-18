namespace Studia.Application.Courses;

public interface ISetCourseCoverImageUseCase
{
    CourseResult Execute(SetCourseCoverImageCommand command);
}
