namespace Studia.Application.Users;

public interface IChangeEmailUseCase
{
    UserResult Execute(ChangeEmailCommand command);
}
