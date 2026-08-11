using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class SetIdentificationUseCaseTests
{
    [Fact]
    public void Execute_SetsTypeAndValueId()
    {
        var users = new FakeUserRepository();
        var user = User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana");
        users.Save(user);
        var useCase = new SetIdentificationUseCase(users);

        var result = useCase.Execute(new SetIdentificationCommand(user.Id, IdentificationType.CC, "1234567890"));

        Assert.Equal(IdentificationType.CC, result.TypeId);
        Assert.Equal("1234567890", result.ValueId);
        Assert.Equal(IdentificationType.CC, users.GetById(user.Id)!.TypeId);
    }

    [Fact]
    public void Execute_WhenUserDoesNotExist_Throws()
    {
        var useCase = new SetIdentificationUseCase(new FakeUserRepository());

        Assert.Throws<InvalidOperationException>(
            () => useCase.Execute(new SetIdentificationCommand(Guid.NewGuid(), IdentificationType.CC, "123")));
    }
}
