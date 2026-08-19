using Studia.Domain.Users;

namespace Studia.Application.Activities;

public record GetActivityByIdQuery(Guid ActivityId, Guid RequestingUserId, Role RequestingUserRole);
