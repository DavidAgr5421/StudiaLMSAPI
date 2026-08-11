namespace Studia.Application.Users;

public record ChangeEmailCommand(Guid UserId, string NewEmail, string CurrentPassword);
