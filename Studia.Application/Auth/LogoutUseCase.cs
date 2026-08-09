using Studia.Domain.Auth;

namespace Studia.Application.Auth;

public class LogoutUseCase(IJwtTokenService jwtTokenService, IRevokedTokenRepository revokedTokenRepository) : ILogoutUseCase
{
    public void Execute(LogoutCommand command)
    {
        var decoded = jwtTokenService.Decode(command.Token)
            ?? throw new InvalidOperationException("Token inválido o expirado.");

        var revokedToken = RevokedToken.Create(decoded.Jti, decoded.ExpiresAtUtc);

        revokedTokenRepository.Save(revokedToken);
    }
}
