namespace Studia.Application.Enrollments;

public record EnrollByInvitationCommand(string InvitationCode, Guid StudentId);
