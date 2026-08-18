using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Studia.Domain.Courses;

public partial class Course
{
    private const string InvitationCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int InvitationCodeLength = 8;
    public const long MaxCoverImageSizeBytes = 5 * 1024 * 1024;

    public Guid Id { get; }
    public string Name { get; private set; }
    public EnrollmentMode EnrollmentMode { get; }
    public CourseStatus Status { get; private set; }
    public string InvitationCode { get; }
    public Guid ProfesorId { get; }

    // Personalización visual: ambas opcionales, el curso funciona sin ellas.
    public string? Color { get; private set; }
    public string? CoverImageFileName { get; private set; }
    public string? CoverImageStorageKey { get; private set; }
    public long? CoverImageSizeBytes { get; private set; }

    private Course(Guid id, string name, EnrollmentMode enrollmentMode, string invitationCode, Guid profesorId)
    {
        Id = id;
        Name = name;
        EnrollmentMode = enrollmentMode;
        Status = CourseStatus.Activo;
        InvitationCode = invitationCode;
        ProfesorId = profesorId;
    }

    public static Course Create(string name, EnrollmentMode enrollmentMode, Guid profesorId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del curso no puede estar vacío.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("El nombre del curso no puede superar los 150 caracteres.", nameof(name));

        if (profesorId == Guid.Empty)
            throw new ArgumentException("El curso debe pertenecer a un profesor.", nameof(profesorId));

        // Independiente del modo: cualquier curso puede compartirse por código de
        // invitación, que siempre pasa por alto el modo (auto-servicio o aprobación).
        var invitationCode = RandomNumberGenerator.GetString(InvitationCodeAlphabet, InvitationCodeLength);

        return new Course(Guid.NewGuid(), name.Trim(), enrollmentMode, invitationCode, profesorId);
    }

    public void Archive()
    {
        if (Status == CourseStatus.Archivado)
            throw new InvalidOperationException($"El curso '{Name}' ya está archivado.");

        Status = CourseStatus.Archivado;
    }

    // Null limpia el color (vuelve al degradé por defecto del front).
    public void UpdateColor(string? color)
    {
        if (color is not null && !HexColorFormat().IsMatch(color))
            throw new ArgumentException("El color debe ser un código hexadecimal válido (ej. #7C3AED).", nameof(color));

        Color = color;
    }

    public void SetCoverImage(string fileName, string storageKey, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("El nombre del archivo no puede estar vacío.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("La referencia de almacenamiento no puede estar vacía.", nameof(storageKey));

        if (sizeBytes <= 0)
            throw new ArgumentException("El tamaño del archivo debe ser mayor a cero.", nameof(sizeBytes));

        if (sizeBytes > MaxCoverImageSizeBytes)
            throw new ArgumentException($"La imagen supera el límite de {MaxCoverImageSizeBytes / (1024 * 1024)}MB.", nameof(sizeBytes));

        CoverImageFileName = fileName.Trim();
        CoverImageStorageKey = storageKey;
        CoverImageSizeBytes = sizeBytes;
    }

    public void RemoveCoverImage()
    {
        CoverImageFileName = null;
        CoverImageStorageKey = null;
        CoverImageSizeBytes = null;
    }

    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColorFormat();
}
