namespace Studia.Application.Auth;

public class ValidateTokenUseCase(IJwtTokenService jwtTokenService, IRevokedTokenRepository revokedTokenRepository) : IValidateTokenUseCase
{
    public ValidateTokenResult Execute(ValidateTokenCommand command)
    {
        var decoded = jwtTokenService.Decode(command.Token)
            ?? throw new InvalidOperationException("Token inválido o expirado.");

        if (revokedTokenRepository.GetByJti(decoded.Jti) is not null)
            throw new InvalidOperationException("La sesión fue cerrada. Inicie sesión nuevamente.");

        return new ValidateTokenResult(decoded.UserId, decoded.Email, decoded.Role);
    }
}
