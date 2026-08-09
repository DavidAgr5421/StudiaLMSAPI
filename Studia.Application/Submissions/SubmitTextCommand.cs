namespace Studia.Application.Submissions;

public record SubmitTextCommand(Guid ActivityId, Guid StudentId, string TextContent);
