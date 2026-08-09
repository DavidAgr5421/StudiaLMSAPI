using Studia.Domain.Auth;

namespace Studia.Domain.Tests.Auth;

public class RevokedTokenTests
{
    [Fact]
    public void Create_WithValidData_SetsFields()
    {
        var expiresAtUtc = DateTime.UtcNow.AddHours(1);

        var revokedToken = RevokedToken.Create("some-jti", expiresAtUtc);

        Assert.Equal("some-jti", revokedToken.Jti);
        Assert.Equal(expiresAtUtc, revokedToken.ExpiresAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankJti_Throws(string blankJti)
    {
        Assert.Throws<ArgumentException>(() => RevokedToken.Create(blankJti, DateTime.UtcNow.AddHours(1)));
    }
}
