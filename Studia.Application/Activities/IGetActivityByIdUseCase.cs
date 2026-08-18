namespace Studia.Application.Activities;

public interface IGetActivityByIdUseCase
{
    ActivityResult? Execute(GetActivityByIdQuery query);
}
