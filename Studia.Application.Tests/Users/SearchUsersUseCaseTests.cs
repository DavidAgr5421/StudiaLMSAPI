using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class SearchUsersUseCaseTests
{
    [Fact]
    public void Execute_MatchesByPartialName()
    {
        var repository = new FakeUserRepository();
        repository.Save(User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana Gómez"));
        repository.Save(User.Register(Email.Create("luis@sena.edu.co"), "hash", Role.Estudiante, "Luis Pérez"));
        var useCase = new SearchUsersUseCase(repository);

        var results = useCase.Execute(new SearchUsersQuery("ana"));

        var result = Assert.Single(results);
        Assert.Equal("ana@sena.edu.co", result.Email);
    }

    [Fact]
    public void Execute_MatchesByPartialEmail()
    {
        var repository = new FakeUserRepository();
        repository.Save(User.Register(Email.Create("profe.ingles@sena.edu.co"), "hash", Role.Profesor, "Carlos Ruiz"));
        var useCase = new SearchUsersUseCase(repository);

        var results = useCase.Execute(new SearchUsersQuery("ingles"));

        Assert.Single(results);
    }

    [Fact]
    public void Execute_WithNoMatches_ReturnsEmpty()
    {
        var repository = new FakeUserRepository();
        repository.Save(User.Register(Email.Create("ana@sena.edu.co"), "hash", Role.Estudiante, "Ana Gómez"));
        var useCase = new SearchUsersUseCase(repository);

        var results = useCase.Execute(new SearchUsersQuery("no-existe"));

        Assert.Empty(results);
    }
}
