using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public record CreateActivityCommand(
    Guid SectionId,
    string Title,
    string Description,
    DateTime DueDateUtc,
    ActivityType Type,
    int? MaxFiles);
