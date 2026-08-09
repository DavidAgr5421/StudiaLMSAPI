using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class RegisterUserUseCaseTests
{
    [Fact]
    public void Execute_WithNewEmail_HashesPasswordAndSaves()
    {
        var repository = new FakeUserRepository();
        var useCase = new RegisterUserUseCase(repository, new FakePasswordHasher());

        var result = useCase.Execute(new RegisterUserCommand("profe@sena.edu.co", "secreta123", Role.Profesor));

        var savedUser = Assert.Single(repository.SavedUsers);
        Assert.Equal(result.Id, savedUser.Id);
        Assert.Equal("hashed:secreta123", savedUser.PasswordHash);
    }

    [Fact]
    public void Execute_WithAlreadyRegisteredEmail_Throws()
    {
        var repository = new FakeUserRepository();
        var useCase = new RegisterUserUseCase(repository, new FakePasswordHasher());
        useCase.Execute(new RegisterUserCommand("profe@sena.edu.co", "secreta123", Role.Profesor));

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(new RegisterUserCommand("Profe@SENA.edu.co", "otra-clave", Role.Estudiante)));
    }

    [Fact]
    public void Execute_WithInvalidEmail_Throws()
    {
        var useCase = new RegisterUserUseCase(new FakeUserRepository(), new FakePasswordHasher());

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(new RegisterUserCommand("no-es-un-email", "secreta123", Role.Estudiante)));
    }
}
