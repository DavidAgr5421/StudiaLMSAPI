namespace Studia.Application.Courses;

public interface IRemoveCourseCoverImageUseCase
{
    CourseResult Execute(RemoveCourseCoverImageCommand command);
}
