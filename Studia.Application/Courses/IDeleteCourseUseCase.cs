namespace Studia.Application.Courses;

public interface IDeleteCourseUseCase
{
    void Execute(DeleteCourseCommand command);
}
