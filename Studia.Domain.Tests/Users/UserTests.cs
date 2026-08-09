using Studia.Domain.Users;

namespace Studia.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Register_WithoutName_LeavesNameNull()
    {
        var email = Email.Create("estudiante@sena.edu.co");

        var user = User.Register(email, passwordHash: "hashed-value", Role.Estudiante);

        Assert.Null(user.Name);
        Assert.Equal(Role.Estudiante, user.Role);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void Register_WithSurroundingWhitespaceInName_TrimsIt()
    {
        var email = Email.Create("profe@sena.edu.co");

        var user = User.Register(email, passwordHash: "hashed-value", Role.Profesor, name: "  María Pérez  ");

        Assert.Equal("María Pérez", user.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Register_WithBlankPasswordHash_Throws(string blankHash)
    {
        var email = Email.Create("profe@sena.edu.co");

        Assert.Throws<ArgumentException>(() => User.Register(email, blankHash, Role.Profesor));
    }
}
