namespace Studia.Application.Activities;

public interface ICloseActivityUseCase
{
    ActivityResult Execute(CloseActivityCommand command);
}
