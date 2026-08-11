namespace Studia.Application.Users;

public interface IUpdateNameUseCase
{
    UserResult Execute(UpdateNameCommand command);
}
