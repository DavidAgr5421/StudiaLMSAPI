namespace Studia.Application.Courses;

public record SetCourseCoverImageCommand(Guid CourseId, string FileName, byte[] Content);
