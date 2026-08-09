using Studia.Domain.Users;

namespace Studia.Application.Auth;

public record DecodedToken(Guid UserId, string Email, Role Role, string Jti, DateTime ExpiresAtUtc);
