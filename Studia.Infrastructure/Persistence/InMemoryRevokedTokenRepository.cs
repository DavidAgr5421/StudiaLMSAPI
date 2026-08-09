using System.Collections.Concurrent;
using Studia.Application.Auth;
using Studia.Domain.Auth;

namespace Studia.Infrastructure.Persistence;

public class InMemoryRevokedTokenRepository : IRevokedTokenRepository
{
    private readonly ConcurrentDictionary<string, RevokedToken> _revokedTokens = new();

    public void Save(RevokedToken revokedToken) => _revokedTokens[revokedToken.Jti] = revokedToken;

    public RevokedToken? GetByJti(string jti) => _revokedTokens.GetValueOrDefault(jti);
}
