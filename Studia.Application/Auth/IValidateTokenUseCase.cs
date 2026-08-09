namespace Studia.Application.Auth;

public interface IValidateTokenUseCase
{
    ValidateTokenResult Execute(ValidateTokenCommand command);
}
