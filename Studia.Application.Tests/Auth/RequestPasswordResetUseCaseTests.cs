using Studia.Application.Auth;
using Studia.Application.Tests.Notifications;
using Studia.Application.Tests.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class RequestPasswordResetUseCaseTests
{
    [Fact]
    public void Execute_WithRegisteredEmail_SavesTokenAndSendsEmail()
    {
        var users = new FakeUserRepository();
        var user = User.Register(Email.Create("ana@sena.edu.co"), "hashed-value", Role.Estudiante, "Ana");
        users.Save(user);
        var tokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new RequestPasswordResetUseCase(users, tokens, emailSender);

        useCase.Execute(new RequestPasswordResetCommand("ana@sena.edu.co"));

        var token = Assert.Single(tokens.SavedTokens);
        Assert.Equal(user.Id, token.UserId);
        Assert.True(token.IsValid(DateTime.UtcNow));
        Assert.Single(emailSender.SentEmails);
    }

    [Fact]
    public void Execute_WithUnknownEmail_DoesNothing()
    {
        var tokens = new FakePasswordResetTokenRepository();
        var emailSender = new FakeEmailSender();
        var useCase = new RequestPasswordResetUseCase(new FakeUserRepository(), tokens, emailSender);

        useCase.Execute(new RequestPasswordResetCommand("no-existe@sena.edu.co"));

        Assert.Empty(tokens.SavedTokens);
        Assert.Empty(emailSender.SentEmails);
    }
}
