using Studia.Application.Sections;
using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public class CreateActivityUseCase(
    IActivityRepository activityRepository,
    ISectionRepository sectionRepository) : ICreateActivityUseCase
{
    public ActivityResult Execute(CreateActivityCommand command)
    {
        _ = sectionRepository.GetById(command.SectionId)
            ?? throw new InvalidOperationException($"No existe una sección con id '{command.SectionId}'.");

        var activity = Activity.Create(
            command.SectionId,
            command.Title,
            command.Description,
            command.DueDateUtc,
            command.Type,
            command.MaxFiles);

        activityRepository.Save(activity);

        return ActivityResult.FromDomain(activity);
    }
}
