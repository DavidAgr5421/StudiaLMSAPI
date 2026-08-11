using Studia.Domain.Users;

namespace Studia.Application.Users;

public record SetIdentificationCommand(Guid UserId, IdentificationType TypeId, string ValueId);
