using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Auth;

public class LoginUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : ILoginUseCase
{
    private const string InvalidCredentialsMessage = "Credenciales inválidas.";

    public LoginResult Execute(LoginCommand command)
    {
        var user = TryFindUserByEmail(command.Email);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new InvalidOperationException(InvalidCredentialsMessage);

        var generated = jwtTokenService.Generate(user.Id, user.Email.Value, user.Role);

        return new LoginResult(user.Id, user.Email.Value, user.Role, generated.Token, generated.ExpiresAtUtc);
    }

    private User? TryFindUserByEmail(string rawEmail)
    {
        try
        {
            return userRepository.GetByEmail(Email.Create(rawEmail));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
