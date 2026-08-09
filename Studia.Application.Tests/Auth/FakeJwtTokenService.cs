using Studia.Application.Auth;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Auth;

public class FakeJwtTokenService : IJwtTokenService
{
    private readonly Dictionary<string, DecodedToken> _tokens = new();

    public GeneratedToken Generate(Guid userId, string email, Role role)
    {
        var jti = Guid.NewGuid().ToString();
        var token = $"fake-token-{jti}";
        var expiresAtUtc = DateTime.UtcNow.AddHours(1);

        _tokens[token] = new DecodedToken(userId, email, role, jti, expiresAtUtc);

        return new GeneratedToken(token, jti, expiresAtUtc);
    }

    public DecodedToken? Decode(string token) => _tokens.GetValueOrDefault(token);
}
