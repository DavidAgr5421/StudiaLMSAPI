using System.Security.Cryptography;
using System.Text;

namespace Studia.Application.Auth;

internal static class PasswordResetTokenHasher
{
    public static string Hash(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
}
