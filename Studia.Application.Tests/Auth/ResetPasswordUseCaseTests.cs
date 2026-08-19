using System.Security.Cryptography;
using System.Text;
using Studia.Application.Auth;
using Studia.Application.Tests.Users;
using Studia.Domain.Auth;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class ResetPasswordUseCaseTests
{
    private static string HashToken(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    private static (FakeUserRepository Users, FakePasswordResetTokenRepository Tokens, FakePasswordHasher Hasher, ResetPasswordUseCase UseCase) CreateSut()
    {
        var users = new FakeUserRepository();
        var tokens = new FakePasswordResetTokenRepository();
        var hasher = new FakePasswordHasher();
        return (users, tokens, hasher, new ResetPasswordUseCase(tokens, users, hasher));
    }

    [Fact]
    public void Execute_WithValidToken_ChangesPasswordAndMarksTokenUsed()
    {
        var (users, tokens, hasher, useCase) = CreateSut();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("oldSecret1"), Role.Estudiante);
        users.Save(user);

        const string rawToken = "raw-token-value";
        var token = PasswordResetToken.Create(user.Id, HashToken(rawToken), DateTime.UtcNow.AddHours(1));
        tokens.Save(token);

        useCase.Execute(new ResetPasswordCommand(rawToken, "newSecret456"));

        Assert.True(hasher.Verify("newSecret456", users.GetById(user.Id)!.PasswordHash));
        Assert.NotNull(tokens.SavedTokens.Single().UsedAtUtc);
    }

    [Fact]
    public void Execute_WithUnknownToken_Throws()
    {
        var (_, _, _, useCase) = CreateSut();

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new ResetPasswordCommand("does-not-exist", "newSecret456")));
    }

    [Fact]
    public void Execute_WithExpiredToken_Throws()
    {
        var (users, tokens, hasher, useCase) = CreateSut();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("oldSecret1"), Role.Estudiante);
        users.Save(user);

        const string rawToken = "raw-token-value";
        var token = PasswordResetToken.Create(user.Id, HashToken(rawToken), DateTime.UtcNow.AddMinutes(-1));
        tokens.Save(token);

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new ResetPasswordCommand(rawToken, "newSecret456")));
    }

    [Fact]
    public void Execute_WithAlreadyUsedToken_Throws()
    {
        var (users, tokens, hasher, useCase) = CreateSut();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("oldSecret1"), Role.Estudiante);
        users.Save(user);

        const string rawToken = "raw-token-value";
        var token = PasswordResetToken.Create(user.Id, HashToken(rawToken), DateTime.UtcNow.AddHours(1));
        token.MarkUsed();
        tokens.Save(token);

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new ResetPasswordCommand(rawToken, "newSecret456")));
    }

    [Fact]
    public void Execute_WithTooShortNewPassword_Throws()
    {
        var (users, tokens, hasher, useCase) = CreateSut();
        var user = User.Register(Email.Create("ana@sena.edu.co"), hasher.Hash("oldSecret1"), Role.Estudiante);
        users.Save(user);

        const string rawToken = "raw-token-value";
        tokens.Save(PasswordResetToken.Create(user.Id, HashToken(rawToken), DateTime.UtcNow.AddHours(1)));

        Assert.Throws<ArgumentException>(() => useCase.Execute(new ResetPasswordCommand(rawToken, "abc")));
    }
}
