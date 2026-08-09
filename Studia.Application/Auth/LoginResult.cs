using Studia.Domain.Users;

namespace Studia.Application.Auth;

public record LoginResult(Guid UserId, string Email, Role Role, string Token, DateTime ExpiresAtUtc);
