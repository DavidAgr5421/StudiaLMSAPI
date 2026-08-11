namespace Studia.Application.Users;

public interface IChangePasswordUseCase
{
    void Execute(ChangePasswordCommand command);
}
