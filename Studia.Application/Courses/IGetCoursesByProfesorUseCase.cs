namespace Studia.Application.Courses;

public interface IGetCoursesByProfesorUseCase
{
    IReadOnlyCollection<CourseResult> Execute(GetCoursesByProfesorQuery query);
}
