using Studia.Application.Submissions;

namespace Studia.Application.Courses;

public class GetCourseCoverImageUseCase(ICourseRepository courseRepository, IFileStorage fileStorage) : IGetCourseCoverImageUseCase
{
    public CourseCoverImageContentResult Execute(GetCourseCoverImageQuery query)
    {
        var course = courseRepository.GetById(query.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{query.CourseId}'.");

        if (course.CoverImageStorageKey is null || course.CoverImageFileName is null)
            throw new InvalidOperationException("El curso no tiene imagen de portada.");

        var content = fileStorage.Retrieve(course.CoverImageStorageKey)
            ?? throw new InvalidOperationException("No se pudo encontrar el contenido de la imagen.");

        return new CourseCoverImageContentResult(course.CoverImageFileName, content);
    }
}
