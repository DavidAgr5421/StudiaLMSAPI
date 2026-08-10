namespace Studia.Application.Activities;

public interface IGetActivitiesBySectionUseCase
{
    IReadOnlyCollection<ActivityResult> Execute(GetActivitiesBySectionQuery query);
}
