using Studia.Application.Auth;
using Studia.Application.Tests.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class LoginUseCaseTests
{
    private static (FakeUserRepository Users, FakePasswordHasher Hasher, FakeJwtTokenService Jwt, LoginUseCase UseCase) CreateSut()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var jwt = new FakeJwtTokenService();
        var useCase = new LoginUseCase(users, hasher, jwt);

        return (users, hasher, jwt, useCase);
    }

    [Fact]
    public void Execute_WithValidCredentials_ReturnsToken()
    {
        var (users, hasher, _, useCase) = CreateSut();
        var user = User.Register(Email.Create("profe@sena.edu.co"), hasher.Hash("secreta123"), Role.Profesor, "Ana");
        users.Save(user);

        var result = useCase.Execute(new LoginCommand("profe@sena.edu.co", "secreta123"));

        Assert.Equal(user.Id, result.UserId);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public void Execute_WithWrongPassword_ThrowsGenericMessage()
    {
        var (users, hasher, _, useCase) = CreateSut();
        var user = User.Register(Email.Create("profe@sena.edu.co"), hasher.Hash("secreta123"), Role.Profesor, "Ana");
        users.Save(user);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new LoginCommand("profe@sena.edu.co", "incorrecta")));
        Assert.Equal("Credenciales inválidas.", ex.Message);
    }

    [Fact]
    public void Execute_WithUnknownEmail_ThrowsSameGenericMessage()
    {
        var (_, _, _, useCase) = CreateSut();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new LoginCommand("no-existe@sena.edu.co", "cualquiera")));
        Assert.Equal("Credenciales inválidas.", ex.Message);
    }

    [Fact]
    public void Execute_WithMalformedEmail_ThrowsSameGenericMessage()
    {
        var (_, _, _, useCase) = CreateSut();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new LoginCommand("no-es-un-email", "cualquiera")));
        Assert.Equal("Credenciales inválidas.", ex.Message);
    }
}
