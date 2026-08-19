namespace Studia.Application.Auth;

public record ResetPasswordCommand(string Token, string NewPassword);
