namespace Studia.Application.Users;

public interface IRegisterUserUseCase
{
    UserResult Execute(RegisterUserCommand command);
}
