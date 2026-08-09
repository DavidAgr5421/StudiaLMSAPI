using Studia.Domain.Users;

namespace Studia.Application.Users;

public record RegisterUserCommand(string Email, string Password, Role Role, string? Name = null);
