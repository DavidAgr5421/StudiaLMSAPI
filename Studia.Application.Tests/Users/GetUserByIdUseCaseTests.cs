using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class GetUserByIdUseCaseTests
{
    [Fact]
    public void Execute_WithExistingUser_ReturnsResult()
    {
        var users = new FakeUserRepository();
        var user = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        users.Save(user);
        var useCase = new GetUserByIdUseCase(users);

        var result = useCase.Execute(new GetUserByIdQuery(user.Id));

        Assert.NotNull(result);
        Assert.Equal("ana@sena.edu.co", result!.Email);
    }

    [Fact]
    public void Execute_WhenUserDoesNotExist_ReturnsNull()
    {
        var useCase = new GetUserByIdUseCase(new FakeUserRepository());

        var result = useCase.Execute(new GetUserByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }
}
