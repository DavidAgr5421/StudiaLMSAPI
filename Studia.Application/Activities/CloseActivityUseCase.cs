namespace Studia.Application.Activities;

public class CloseActivityUseCase(IActivityRepository activityRepository) : ICloseActivityUseCase
{
    public ActivityResult Execute(CloseActivityCommand command)
    {
        var activity = activityRepository.GetById(command.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{command.ActivityId}'.");

        activity.Close();
        activityRepository.Save(activity);

        return ActivityResult.FromDomain(activity);
    }
}
