using Studia.Domain.Users;

namespace Studia.Domain.Tests.Users;

public class EmailTests
{
    [Fact]
    public void Create_WithValidAddress_NormalizesToLowercase()
    {
        var email = Email.Create("Profe@SENA.edu.co");

        Assert.Equal("profe@sena.edu.co", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-es-un-email")]
    [InlineData("falta-dominio@")]
    [InlineData("@sena.edu.co")]
    public void Create_WithInvalidAddress_Throws(string invalidAddress)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(invalidAddress));
    }

    [Fact]
    public void TwoEmails_WithSameAddressDifferentCasing_AreEqual()
    {
        var first = Email.Create("profe@sena.edu.co");
        var second = Email.Create("PROFE@sena.edu.co");

        Assert.Equal(first, second);
    }
}
