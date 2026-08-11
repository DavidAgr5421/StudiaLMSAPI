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

    [Fact]
    public void Rename_WithBlankName_SetsNameToNull()
    {
        var user = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor, "María Pérez");

        user.Rename("   ");

        Assert.Null(user.Name);
    }

    [Fact]
    public void Rename_WithSurroundingWhitespace_TrimsIt()
    {
        var user = User.Register(Email.Create("profe@sena.edu.co"), "hashed-value", Role.Profesor);

        user.Rename("  María Pérez  ");

        Assert.Equal("María Pérez", user.Name);
    }

    [Fact]
    public void ChangeEmail_ReplacesTheEmail()
    {
        var user = User.Register(Email.Create("vieja@sena.edu.co"), "hashed-value", Role.Estudiante);
        var newEmail = Email.Create("nueva@sena.edu.co");

        user.ChangeEmail(newEmail);

        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void ChangePassword_ReplacesTheHash()
    {
        var user = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante);

        user.ChangePassword("new-hashed-value");

        Assert.Equal("new-hashed-value", user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePassword_WithBlankHash_Throws(string blankHash)
    {
        var user = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante);

        Assert.Throws<ArgumentException>(() => user.ChangePassword(blankHash));
    }

    [Fact]
    public void SetIdentification_SetsTypeAndValue()
    {
        var user = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante);

        user.SetIdentification(IdentificationType.CC, " 1234567890 ");

        Assert.Equal(IdentificationType.CC, user.TypeId);
        Assert.Equal("1234567890", user.ValueId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SetIdentification_WithBlankValue_Throws(string blankValue)
    {
        var user = User.Register(Email.Create("estudiante@sena.edu.co"), "hashed-value", Role.Estudiante);

        Assert.Throws<ArgumentException>(() => user.SetIdentification(IdentificationType.Pasaporte, blankValue));
    }
}
