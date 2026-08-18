namespace Studia.Application.Courses;

public class UpdateCourseColorUseCase(ICourseRepository courseRepository) : IUpdateCourseColorUseCase
{
    public CourseResult Execute(UpdateCourseColorCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        course.UpdateColor(command.Color);
        courseRepository.Save(course);

        return CourseResult.FromDomain(course);
    }
}
