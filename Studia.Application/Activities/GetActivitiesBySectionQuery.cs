using Studia.Domain.Users;

namespace Studia.Application.Activities;

public record GetActivitiesBySectionQuery(Guid SectionId, Guid RequestingUserId, Role RequestingUserRole);
