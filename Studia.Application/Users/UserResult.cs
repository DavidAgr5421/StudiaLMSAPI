using Studia.Domain.Users;

namespace Studia.Application.Users;

public record UserResult(
    Guid Id,
    string Email,
    string? Name,
    Role Role,
    IdentificationType? TypeId = null,
    string? ValueId = null)
{
    public static UserResult FromDomain(User user) =>
        new(user.Id, user.Email.Value, user.Name, user.Role, user.TypeId, user.ValueId);
}
