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
    public string? InvitationCode { get; }

    private Course(Guid id, string name, EnrollmentMode enrollmentMode, string? invitationCode)
    {
        Id = id;
        Name = name;
        EnrollmentMode = enrollmentMode;
        Status = CourseStatus.Activo;
        InvitationCode = invitationCode;
    }

    public static Course Create(string name, EnrollmentMode enrollmentMode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del curso no puede estar vacío.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("El nombre del curso no puede superar los 150 caracteres.", nameof(name));

        var invitationCode = enrollmentMode == EnrollmentMode.PorInvitacion
            ? RandomNumberGenerator.GetString(InvitationCodeAlphabet, InvitationCodeLength)
            : null;

        return new Course(Guid.NewGuid(), name.Trim(), enrollmentMode, invitationCode);
    }

    public void Archive()
    {
        if (Status == CourseStatus.Archivado)
            throw new InvalidOperationException($"El curso '{Name}' ya está archivado.");

        Status = CourseStatus.Archivado;
    }
}
