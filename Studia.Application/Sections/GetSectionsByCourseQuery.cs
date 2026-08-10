using Studia.Domain.Users;

namespace Studia.Application.Sections;

public record GetSectionsByCourseQuery(Guid CourseId, Guid RequestingUserId, Role RequestingUserRole);
