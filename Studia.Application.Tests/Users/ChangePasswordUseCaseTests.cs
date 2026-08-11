using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class ChangePasswordUseCaseTests
{
    [Fact]
    public void Execute_WithCorrectCurrentPassword_ChangesPassword()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        users.Save(user);
        var useCase = new ChangePasswordUseCase(users, hasher);

        useCase.Execute(new ChangePasswordCommand(user.Id, "secret123", "newSecret456"));

        Assert.True(hasher.Verify("newSecret456", users.GetById(user.Id)!.PasswordHash));
    }

    [Fact]
    public void Execute_WithWrongCurrentPassword_Throws()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        users.Save(user);
        var useCase = new ChangePasswordUseCase(users, hasher);

        Assert.Throws<InvalidOperationException>(
            () => useCase.Execute(new ChangePasswordCommand(user.Id, "wrong", "newSecret456")));
    }

    [Fact]
    public void Execute_WithTooShortNewPassword_Throws()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("secret123"), Role.Estudiante);
        users.Save(user);
        var useCase = new ChangePasswordUseCase(users, hasher);

        Assert.Throws<ArgumentException>(() => useCase.Execute(new ChangePasswordCommand(user.Id, "secret123", "abc")));
    }
}
