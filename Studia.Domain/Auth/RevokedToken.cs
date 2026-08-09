namespace Studia.Domain.Auth;

public class RevokedToken
{
    public string Jti { get; }
    public DateTime ExpiresAtUtc { get; }

    private RevokedToken(string jti, DateTime expiresAtUtc)
    {
        Jti = jti;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static RevokedToken Create(string jti, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(jti))
            throw new ArgumentException("El identificador del token no puede estar vacío.", nameof(jti));

        return new RevokedToken(jti, expiresAtUtc);
    }
}
