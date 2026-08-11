using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class ChangeEmailUseCaseTests
{
    [Fact]
    public void Execute_WithCorrectPassword_ChangesEmail()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante, "Ana");
        users.Save(user);
        var useCase = new ChangeEmailUseCase(users, hasher);

        var result = useCase.Execute(new ChangeEmailCommand(user.Id, "ana.nueva@sena.edu.co", "secret123"));

        Assert.Equal("ana.nueva@sena.edu.co", result.Email);
    }

    [Fact]
    public void Execute_WithWrongPassword_Throws()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        users.Save(user);
        var useCase = new ChangeEmailUseCase(users, hasher);

        Assert.Throws<InvalidOperationException>(
            () => useCase.Execute(new ChangeEmailCommand(user.Id, "ana.nueva@sena.edu.co", "wrong")));
    }

    [Fact]
    public void Execute_WhenNewEmailAlreadyTaken_Throws()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        var other = User.Register(Email.Create("otra@sena.edu.co"), hasher.Hash("otra123"), Role.Estudiante);
        users.Save(user);
        users.Save(other);
        var useCase = new ChangeEmailUseCase(users, hasher);

        Assert.Throws<InvalidOperationException>(
            () => useCase.Execute(new ChangeEmailCommand(user.Id, "otra@sena.edu.co", "secret123")));
    }

    [Fact]
    public void Execute_WhenNewEmailIsTheUsersOwnEmail_DoesNotThrow()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        users.Save(user);
        var useCase = new ChangeEmailUseCase(users, hasher);

        var result = useCase.Execute(new ChangeEmailCommand(user.Id, "ana@sena.edu.co", "secret123"));

        Assert.Equal("ana@sena.edu.co", result.Email);
    }
}
