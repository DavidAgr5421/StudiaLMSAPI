namespace Studia.Application.Users;

public class ChangePasswordUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher) : IChangePasswordUseCase
{
    public void Execute(ChangePasswordCommand command)
    {
        var user = userRepository.GetById(command.UserId)
            ?? throw new InvalidOperationException($"No existe un usuario con id '{command.UserId}'.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("La contraseña actual no es correcta.");

        if (command.NewPassword.Length < 6)
            throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.", nameof(command));

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));
        userRepository.Save(user);
    }
}
