using Studia.Application.Users;

namespace Studia.Application.Auth;

public class ResetPasswordUseCase(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher) : IResetPasswordUseCase
{
    public void Execute(ResetPasswordCommand command)
    {
        if (command.NewPassword.Length < 6)
            throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.", nameof(command));

        var tokenHash = PasswordResetTokenHasher.Hash(command.Token);
        var token = tokenRepository.GetByTokenHash(tokenHash);

        if (token is null || !token.IsValid(DateTime.UtcNow))
            throw new InvalidOperationException("El enlace no es válido o ya venció. Solicitá uno nuevo.");

        var user = userRepository.GetById(token.UserId)
            ?? throw new InvalidOperationException("No existe el usuario asociado a este enlace.");

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));
        userRepository.Save(user);

        token.MarkUsed();
        tokenRepository.Save(token);
    }
}
