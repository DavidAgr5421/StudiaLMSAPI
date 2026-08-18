using Studia.Domain.Courses;

namespace Studia.Application.Courses;

// ProfesorName es opcional por el mismo motivo que EnrollmentResult.StudentName: FromDomain(Course)
// no tiene acceso al User -- harina de otro aggregate. Quien sí lo tenga (GetCourseByIdUseCase) lo
// completa después con una expresión "with".
public record CourseResult(
    Guid Id,
    string Name,
    EnrollmentMode EnrollmentMode,
    CourseStatus Status,
    string InvitationCode,
    Guid ProfesorId,
    string? ProfesorName = null,
    string? Color = null,
    string? CoverImageFileName = null)
{
    public static CourseResult FromDomain(Course course) =>
        new(
            course.Id,
            course.Name,
            course.EnrollmentMode,
            course.Status,
            course.InvitationCode,
            course.ProfesorId,
            Color: course.Color,
            CoverImageFileName: course.CoverImageFileName);
}
