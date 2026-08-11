using System.Security.Claims;
using Studia.Domain.Users;

namespace Studia.WebApi;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst("sub")?.Value
            ?? throw new InvalidOperationException("El token no contiene el identificador del usuario.");

        return Guid.Parse(value);
    }

    public static Role GetRole(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new InvalidOperationException("El token no contiene el rol del usuario.");

        return Enum.Parse<Role>(value);
    }
}
