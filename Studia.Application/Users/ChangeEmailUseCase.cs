using Studia.Domain.Users;

namespace Studia.Application.Users;

public class ChangeEmailUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher) : IChangeEmailUseCase
{
    public UserResult Execute(ChangeEmailCommand command)
    {
        var user = userRepository.GetById(command.UserId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.UserId}'.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("La contraseña actual no es correcta.");

        var newEmail = Email.Create(command.NewEmail);

        var existing = userRepository.GetByEmail(newEmail);
        if (existing is not null && existing.Id != user.Id)
            throw new InvalidOperationException($"Ya existe un usuario registrado con el email '{newEmail}'.");

        user.ChangeEmail(newEmail);
        userRepository.Save(user);

        return UserResult.FromDomain(user);
    }
}
