namespace Studia.Application.Enrollments;

public record AddStudentsToCourseResult(IReadOnlyCollection<AddStudentToCourseOutcome> Outcomes);
