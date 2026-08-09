using Studia.Domain.Users;

namespace Studia.Application.Auth;

public interface IJwtTokenService
{
    GeneratedToken Generate(Guid userId, string email, Role role);

    DecodedToken? Decode(string token);
}
