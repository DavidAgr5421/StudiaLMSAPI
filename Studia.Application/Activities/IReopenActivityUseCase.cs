namespace Studia.Application.Activities;

public interface IReopenActivityUseCase
{
    ActivityResult Execute(ReopenActivityCommand command);
}
