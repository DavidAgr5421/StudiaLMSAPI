namespace Studia.Application.Auth;

public interface ILogoutUseCase
{
    void Execute(LogoutCommand command);
}
