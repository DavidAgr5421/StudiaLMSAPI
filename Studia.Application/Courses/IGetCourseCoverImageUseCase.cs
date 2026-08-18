namespace Studia.Application.Courses;

public interface IGetCourseCoverImageUseCase
{
    CourseCoverImageContentResult Execute(GetCourseCoverImageQuery query);
}
