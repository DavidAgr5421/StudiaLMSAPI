namespace Studia.Application.Activities;

public class ReopenActivityUseCase(IActivityRepository activityRepository) : IReopenActivityUseCase
{
    public ActivityResult Execute(ReopenActivityCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        activity.Reopen();
        activityRepository.Save(activity);

        return ActivityResult.FromDomain(activity);
    }
}
