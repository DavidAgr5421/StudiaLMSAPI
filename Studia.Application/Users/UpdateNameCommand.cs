namespace Studia.Application.Users;

public record UpdateNameCommand(Guid UserId, string? Name);
