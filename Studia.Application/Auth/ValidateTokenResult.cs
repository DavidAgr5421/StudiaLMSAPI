using Studia.Domain.Users;

namespace Studia.Application.Auth;

public record ValidateTokenResult(Guid UserId, string Email, Role Role);
