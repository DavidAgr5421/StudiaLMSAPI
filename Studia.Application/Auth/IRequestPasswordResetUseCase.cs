namespace Studia.Application.Auth;

public interface IRequestPasswordResetUseCase
{
    void Execute(RequestPasswordResetCommand command);
}
