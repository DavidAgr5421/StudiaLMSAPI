namespace Studia.Application.Users;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
