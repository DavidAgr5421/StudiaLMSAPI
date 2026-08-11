namespace Studia.Application.Users;

public class SetIdentificationUseCase(IUserRepository userRepository) : ISetIdentificationUseCase
{
    public UserResult Execute(SetIdentificationCommand command)
    {
        var user = userRepository.GetById(command.UserId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.UserId}'.");

        user.SetIdentification(command.TypeId, command.ValueId);
        userRepository.Save(user);

        return UserResult.FromDomain(user);
    }
}
