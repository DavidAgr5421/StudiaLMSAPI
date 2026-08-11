using Studia.Application.Activities;
using Studia.Application.Submissions;

namespace Studia.Application.Sections;

public class DeleteSectionUseCase(
    ISectionRepository sectionRepository,
    IActivityRepository activityRepository,
    ISubmissionRepository submissionRepository) : IDeleteSectionUseCase
{
    public void Execute(DeleteSectionCommand command)
    {
        var section = sectionRepository.GetById(command.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{command.SectionId}'.");

        // Cascada manual: cada agregado (sección, actividad, entrega) vive en su propia
        // tabla sin FK real entre ellas, así que hay que limpiar de abajo hacia arriba.
        foreach (var activity in activityRepository.GetBySectionId(section.Id))
            submissionRepository.DeleteByActivityId(activity.Id);

        activityRepository.DeleteBySectionId(section.Id);
        sectionRepository.Delete(section.Id);
    }
}
