using Studia.Application.Auth;
using Studia.Domain.Auth;

namespace Studia.Application.Tests.Auth;

public class FakeRevokedTokenRepository : IRevokedTokenRepository
{
    private readonly Dictionary<string, RevokedToken> _revokedTokens = new();

    public void Save(RevokedToken revokedToken) => _revokedTokens[revokedToken.Jti] = revokedToken;

    public RevokedToken? GetByJti(string jti) => _revokedTokens.GetValueOrDefault(jti);
}
