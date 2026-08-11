namespace Studia.Application.Users;

public class GetUserByIdUseCase(IUserRepository userRepository) : IGetUserByIdUseCase
{
    public UserResult? Execute(GetUserByIdQuery query)
    {
        var user = userRepository.GetById(query.UserId);

        return user is null ? null : UserResult.FromDomain(user);
    }
}
