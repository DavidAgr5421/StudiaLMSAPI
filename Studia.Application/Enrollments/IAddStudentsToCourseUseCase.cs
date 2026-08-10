namespace Studia.Application.Enrollments;

public interface IAddStudentsToCourseUseCase
{
    AddStudentsToCourseResult Execute(AddStudentsToCourseCommand command);
}
