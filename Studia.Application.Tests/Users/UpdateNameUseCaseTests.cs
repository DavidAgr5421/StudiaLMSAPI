using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class UpdateNameUseCaseTests
{
    [Fact]
    public void Execute_UpdatesTheUsersName()
    {
        var users = new FakeUserRepository();
        var user = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        users.Save(user);
        var useCase = new UpdateNameUseCase(users);

        var result = useCase.Execute(new UpdateNameCommand(user.Id, "Ana Torres"));

        Assert.Equal("Ana Torres", result.Name);
        Assert.Equal("Ana Torres", users.GetById(user.Id)!.Name);
    }

    [Fact]
    public void Execute_WhenUserDoesNotExist_Throws()
    {
        var useCase = new UpdateNameUseCase(new FakeUserRepository());

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new UpdateNameCommand(Guid.NewGuid(), "Ana")));
    }
}
