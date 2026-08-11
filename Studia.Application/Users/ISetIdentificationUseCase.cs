namespace Studia.Application.Users;

public interface ISetIdentificationUseCase
{
    UserResult Execute(SetIdentificationCommand command);
}
