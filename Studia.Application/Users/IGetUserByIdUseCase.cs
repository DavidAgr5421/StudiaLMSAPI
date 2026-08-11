namespace Studia.Application.Users;

public interface IGetUserByIdUseCase
{
    UserResult? Execute(GetUserByIdQuery query);
}
