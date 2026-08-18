using Studia.Application.Submissions;

namespace Studia.Application.Courses;

public class SetCourseCoverImageUseCase(ICourseRepository courseRepository, IFileStorage fileStorage) : ISetCourseCoverImageUseCase
{
    // No es una regla de dominio (eso es tamaño/nombre, en Course.SetCoverImage) sino una
    // política de la aplicación: qué formatos aceptamos como portada.
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp", ".gif",
    };

    public CourseResult Execute(SetCourseCoverImageCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        var extension = Path.GetExtension(command.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException("La imagen debe ser PNG, JPG, WEBP o GIF.", nameof(command));

        var storageKey = fileStorage.Store(command.FileName, command.Content);
        course.SetCoverImage(command.FileName, storageKey, command.Content.LongLength);
        courseRepository.Save(course);

        return CourseResult.FromDomain(course);
    }
}
