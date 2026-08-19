using Studia.Application.Auth;
using Studia.Domain.Auth;

namespace Studia.Application.Tests.Auth;

public class FakePasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly Dictionary<Guid, PasswordResetToken> _tokens = new();

    public IReadOnlyCollection<PasswordResetToken> SavedTokens => _tokens.Values.ToList();

    public void Save(PasswordResetToken token) => _tokens[token.Id] = token;

    public PasswordResetToken? GetByTokenHash(string tokenHash) =>
        _tokens.Values.FirstOrDefault(t => t.TokenHash == tokenHash);
}
