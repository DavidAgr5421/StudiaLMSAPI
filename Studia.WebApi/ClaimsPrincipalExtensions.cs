using System.Security.Claims;

namespace Studia.WebApi;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst("sub")?.Value
            ?? throw new InvalidOperationException("El token no contiene el identificador del usuario.");

        return Guid.Parse(value);
    }
}
