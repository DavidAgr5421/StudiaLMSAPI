using Studia.Application.Activities;
using Studia.Application.Courses;
using Studia.Application.Sections;

namespace Studia.Application.Submissions;

public class GetSubmissionFileUseCase(
    ISubmissionRepository submissionRepository,
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICourseRepository courseRepository,
    IFileStorage fileStorage) : IGetSubmissionFileUseCase
{
    public SubmissionFileContentResult Execute(GetSubmissionFileQuery query)
    {
        var submission = submissionRepository.GetById(query.SubmissionId)
            ?? throw new InvalidOperationException($"No existe una entrega con id '{query.SubmissionId}'.");

        // Ata el storageKey a ESTA entrega -- igual que con el material de apoyo de una
        // actividad, sin este chequeo cualquier storageKey válido de otra entrega serviría
        // igual, filtrando el trabajo de otro estudiante.
        var file = submission.Files.FirstOrDefault(f => f.StorageKey == query.StorageKey)
            ?? throw new InvalidOperationException("El archivo no pertenece a esta entrega.");

        var isOwner = submission.StudentId == query.RequestingUserId;
        if (!isOwner && !query.RequestingUserIsAdmin && !IsProfesorOfCourse(submission.ActivityId, query.RequestingUserId))
            throw new InvalidOperationException("No tiene permiso para descargar este archivo.");

        var content = fileStorage.Retrieve(file.StorageKey)
            ?? throw new InvalidOperationException("No se pudo encontrar el contenido del archivo.");

        return new SubmissionFileContentResult(file.FileName, content);
    }

    private bool IsProfesorOfCourse(Guid activityId, Guid requestingUserId)
    {
        var activity = activityRepository.GetById(activityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{activityId}'.");

        var section = sectionRepository.GetById(activity.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{activity.SectionId}'.");

        var course = courseRepository.GetById(section.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{section.CourseId}'.");

        return course.ProfesorId == requestingUserId;
    }
}
