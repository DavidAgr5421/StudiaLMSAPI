namespace Studia.Application.Auth;

public interface ILoginUseCase
{
    LoginResult Execute(LoginCommand command);
}
