namespace Studia.Application.Courses;

public class RemoveCourseCoverImageUseCase(ICourseRepository courseRepository) : IRemoveCourseCoverImageUseCase
{
    public CourseResult Execute(RemoveCourseCoverImageCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        course.RemoveCoverImage();
        courseRepository.Save(course);

        return CourseResult.FromDomain(course);
    }
}
