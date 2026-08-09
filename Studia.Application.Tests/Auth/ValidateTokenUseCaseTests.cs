using Studia.Application.Auth;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class ValidateTokenUseCaseTests
{
    [Fact]
    public void Execute_WithValidNonRevokedToken_ReturnsClaims()
    {
        var jwt = new FakeJwtTokenService();
        var repository = new FakeRevokedTokenRepository();
        var userId = Guid.NewGuid();
        var generated = jwt.Generate(userId, "profe@sena.edu.co", Role.Profesor);
        var useCase = new ValidateTokenUseCase(jwt, repository);

        var result = useCase.Execute(new ValidateTokenCommand(generated.Token));

        Assert.Equal(userId, result.UserId);
        Assert.Equal(Role.Profesor, result.Role);
    }

    [Fact]
    public void Execute_WithRevokedToken_Throws()
    {
        var jwt = new FakeJwtTokenService();
        var repository = new FakeRevokedTokenRepository();
        var generated = jwt.Generate(Guid.NewGuid(), "profe@sena.edu.co", Role.Profesor);
        var logoutUseCase = new LogoutUseCase(jwt, repository);
        logoutUseCase.Execute(new LogoutCommand(generated.Token));
        var useCase = new ValidateTokenUseCase(jwt, repository);

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new ValidateTokenCommand(generated.Token)));
    }

    [Fact]
    public void Execute_WithInvalidToken_Throws()
    {
        var useCase = new ValidateTokenUseCase(new FakeJwtTokenService(), new FakeRevokedTokenRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new ValidateTokenCommand("no-existe")));
    }
}
