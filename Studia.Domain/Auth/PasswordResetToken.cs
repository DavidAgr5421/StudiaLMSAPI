namespace Studia.Domain.Auth;

// Guarda el hash del token, no el valor crudo -- igual criterio que las contraseñas: si
// se filtra la base, nadie puede usar estas filas para resetear una cuenta ajena.
public class PasswordResetToken
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string TokenHash { get; }
    public DateTime ExpiresAtUtc { get; }
    public DateTime? UsedAtUtc { get; private set; }

    private PasswordResetToken(Guid id, Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("El usuario no es válido.", nameof(userId));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("El hash del token no puede estar vacío.", nameof(tokenHash));

        return new PasswordResetToken(Guid.NewGuid(), userId, tokenHash, expiresAtUtc);
    }

    public bool IsValid(DateTime nowUtc) => UsedAtUtc is null && nowUtc <= ExpiresAtUtc;

    public void MarkUsed()
    {
        if (UsedAtUtc is not null)
            throw new InvalidOperationException("Este enlace ya fue usado.");

        UsedAtUtc = DateTime.UtcNow;
    }
}
