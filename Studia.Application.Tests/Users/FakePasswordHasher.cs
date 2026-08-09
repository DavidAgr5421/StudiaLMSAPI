using Studia.Application.Users;

namespace Studia.Application.Tests.Users;

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword) => $"hashed:{plainPassword}";

    public bool Verify(string plainPassword, string passwordHash) => passwordHash == Hash(plainPassword);
}
