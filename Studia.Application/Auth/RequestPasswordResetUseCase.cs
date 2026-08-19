using System.Security.Cryptography;
using Studia.Application.Notifications;
using Studia.Application.Users;
using Studia.Domain.Auth;
using Studia.Domain.Users;

namespace Studia.Application.Auth;

public class RequestPasswordResetUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenRepository tokenRepository,
    IEmailSender emailSender) : IRequestPasswordResetUseCase
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    public void Execute(RequestPasswordResetCommand command)
    {
        User? user;
        try
        {
            user = userRepository.GetByEmail(Email.Create(command.Email));
        }
        catch (ArgumentException)
        {
            user = null;
        }

        // Sin importar si el email existe o no, se responde igual afuera (ver endpoint) --
        // si acá se cortara distinto, cualquiera podría usar este endpoint para
        // averiguar qué emails están registrados.
        if (user is null)
            return;

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var tokenHash = PasswordResetTokenHasher.Hash(rawToken);
        var token = PasswordResetToken.Create(user.Id, tokenHash, DateTime.UtcNow.Add(TokenLifetime));

        tokenRepository.Save(token);

        emailSender.Send(
            user.Email.Value,
            "Restablecer tu contraseña de Studia",
            $"Usá este código para restablecer tu contraseña (válido por 1 hora): {rawToken}");
    }
}
