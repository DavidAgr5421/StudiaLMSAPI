namespace Studia.Application.Enrollments;

public interface IEnrollStudentInOpenCourseUseCase
{
    EnrollmentResult Execute(EnrollStudentInOpenCourseCommand command);
}
