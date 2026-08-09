using Studia.Domain.Users;

namespace Studia.Application.Users;

public record UserResult(Guid Id, string Email, string? Name, Role Role)
{
    public static UserResult FromDomain(User user) =>
        new(user.Id, user.Email.Value, user.Name, user.Role);
}
