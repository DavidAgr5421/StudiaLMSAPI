namespace Studia.Domain.Users;

public class User
{
    public Guid Id { get; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? Name { get; private set; }
    public Role Role { get; }
    public IdentificationType? TypeId { get; private set; }
    public string? ValueId { get; private set; }

    private User(Guid id, Email email, string passwordHash, Role role, string? name)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Name = name;
    }

    public static User Register(Email email, string passwordHash, Role role, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña no puede estar vacío.", nameof(passwordHash));

        return new User(Guid.NewGuid(), email, passwordHash, role, string.IsNullOrWhiteSpace(name) ? null : name.Trim());
    }

    public void Rename(string? name) => Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

    public void ChangeEmail(Email newEmail) => Email = newEmail;

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("El hash de la contraseña no puede estar vacío.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public void SetIdentification(IdentificationType typeId, string valueId)
    {
        if (string.IsNullOrWhiteSpace(valueId))
            throw new ArgumentException("El número de identificación no puede estar vacío.", nameof(valueId));

        TypeId = typeId;
        ValueId = valueId.Trim();
    }
}
