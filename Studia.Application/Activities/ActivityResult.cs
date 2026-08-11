using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public record ActivityResult(
    Guid Id,
    Guid SectionId,
    string Title,
    string Description,
    DateTime DueDateUtc,
    ActivityType Type,
    int? MaxFiles,
    IReadOnlyCollection<Guid> CohortIds,
    IReadOnlyCollection<ActivityFileResult> Files)
{
    public static ActivityResult FromDomain(Activity activity) =>
        new(
            activity.Id,
            activity.SectionId,
            activity.Title,
            activity.Description,
            activity.DueDateUtc,
            activity.Type,
            activity.MaxFiles,
            activity.CohortIds,
            activity.Files.Select(ActivityFileResult.FromDomain).ToList());
}
