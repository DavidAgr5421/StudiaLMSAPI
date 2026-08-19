using Studia.Domain.Auth;

namespace Studia.Application.Auth;

public interface IPasswordResetTokenRepository
{
    void Save(PasswordResetToken token);

    PasswordResetToken? GetByTokenHash(string tokenHash);
}
