namespace Studia.Domain.Users;

public class User
{
    public Guid Id { get; }
    public Email Email { get; }
    public string PasswordHash { get; }
    public string? Name { get; private set; }
    public Role Role { get; }

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
}
