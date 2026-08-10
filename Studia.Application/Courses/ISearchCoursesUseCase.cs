namespace Studia.Application.Courses;

public interface ISearchCoursesUseCase
{
    IReadOnlyCollection<CourseResult> Execute(SearchCoursesQuery query);
}
