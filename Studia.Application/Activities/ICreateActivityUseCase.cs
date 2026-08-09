namespace Studia.Application.Activities;

public interface ICreateActivityUseCase
{
    ActivityResult Execute(CreateActivityCommand command);
}
