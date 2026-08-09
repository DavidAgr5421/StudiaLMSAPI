using Studia.Application.Auth;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class LogoutUseCaseTests
{
    [Fact]
    public void Execute_WithValidToken_RevokesIt()
    {
        var jwt = new FakeJwtTokenService();
        var repository = new FakeRevokedTokenRepository();
        var generated = jwt.Generate(Guid.NewGuid(), "profe@sena.edu.co", Role.Profesor);
        var useCase = new LogoutUseCase(jwt, repository);

        useCase.Execute(new LogoutCommand(generated.Token));

        Assert.NotNull(repository.GetByJti(generated.Jti));
    }

    [Fact]
    public void Execute_WithInvalidToken_Throws()
    {
        var useCase = new LogoutUseCase(new FakeJwtTokenService(), new FakeRevokedTokenRepository());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new LogoutCommand("token-inexistente")));
    }
}
