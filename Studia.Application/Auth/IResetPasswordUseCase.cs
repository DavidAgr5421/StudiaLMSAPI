namespace Studia.Application.Auth;

public interface IResetPasswordUseCase
{
    void Execute(ResetPasswordCommand command);
}
