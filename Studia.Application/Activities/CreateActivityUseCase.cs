using Studia.Application.Cohorts;
using Studia.Application.Sections;
using Studia.Application.Submissions;
using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public class CreateActivityUseCase(
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository,
    ICohortRepository cohortRepository,
    IFileStorage fileStorage,
    IHtmlSanitizer htmlSanitizer) : ICreateActivityUseCase
{
    public ActivityResult Execute(CreateActivityCommand command)
    {
        var section = sectionRepository.GetById(command.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{command.SectionId}'.");

        var cohortIds = command.CohortIds ?? [];
        foreach (var cohortId in cohortIds)
        {
            var cohort = cohortRepository.GetById(cohortId)
                ?? throw new InvalidOperationException($"No existe una ficha con id '{cohortId}'.");

            if (cohort.CourseId != section.CourseId)
                throw new InvalidOperationException($"La ficha '{cohort.Name}' no pertenece a este curso.");
        }

        // El tamaño máximo por archivo (10MB) lo valida ActivityFile.Create -- acá solo
        // se guarda el contenido y se arma la referencia.
        var files = (command.Files ?? [])
            .Select(file =>
            {
                var storageKey = fileStorage.Store(file.FileName, file.Content);
                return ActivityFile.Create(file.FileName, storageKey, file.Content.Length);
            })
            .ToList();

        // Igual que la descripción de una sección: admite el mismo HTML enriquecido
        // (negrita, encabezados, justificado) que produce el editor del profesor.
        var sanitizedDescription = htmlSanitizer.Sanitize(command.Description);

        var activity = Activity.Create(
            command.SectionId,
            command.Title,
            sanitizedDescription,
            command.DueDateUtc,
            command.Type,
            command.MaxFiles,
            cohortIds,
            files,
            command.Status,
            command.Kind,
            command.OpenDateUtc,
            command.AllowsLateSubmission);

        activityRepository.Save(activity);

        return ActivityResult.FromDomain(activity);
    }
}
