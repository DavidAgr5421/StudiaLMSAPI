using System.Text.RegularExpressions;

namespace Studia.Domain.Users;

public sealed partial class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El email no puede estar vacío.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailFormat().IsMatch(normalized))
            throw new ArgumentException($"El email '{value}' no tiene un formato válido.", nameof(value));

        return new Email(normalized);
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as Email);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailFormat();
}
