using Studia.Domain.Auth;

namespace Studia.Application.Auth;

public interface IRevokedTokenRepository
{
    void Save(RevokedToken revokedToken);

    RevokedToken? GetByJti(string jti);
}
