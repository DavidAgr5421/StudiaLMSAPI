using System.Security.Cryptography;

namespace Studia.Domain.Courses;

public class Course
{
    private const string InvitationCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int InvitationCodeLength = 8;

    public Guid Id { get; }
    public string Name { get; private set; }
    public EnrollmentMode EnrollmentMode { get; }
    public CourseStatus Status { get; private set; }
    public string InvitationCode { get; }
    public Guid ProfesorId { get; }

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
}
