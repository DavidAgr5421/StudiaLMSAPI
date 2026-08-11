namespace Studia.Application.Users;

public class UpdateNameUseCase(IUserRepository userRepository) : IUpdateNameUseCase
{
    public UserResult Execute(UpdateNameCommand command)
    {
        var user = userRepository.GetById(command.UserId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.UserId}'.");

        user.Rename(command.Name);
        userRepository.Save(user);

        return UserResult.FromDomain(user);
    }
}
