using Studia.Application.Cohorts;
using Studia.Application.Enrollments;
using Studia.Application.Sections;

namespace Studia.Application.Courses;

public class DeleteCourseUseCase(
    ICourseRepository courseRepository,
    ISectionRepository sectionRepository,
    IDeleteSectionUseCase deleteSectionUseCase,
    IEnrollmentRepository enrollmentRepository,
    ICohortRepository cohortRepository) : IDeleteCourseUseCase
{
    public void Execute(DeleteCourseCommand command)
    {
        var course = courseRepository.GetById(command.CourseId)
            ?? throw new InvalidOperationException($"No existe un curso con id '{command.CourseId}'.");

        // Reusa la cascada de DeleteSectionUseCase para no duplicar la lógica de borrar
        // actividades y entregas -- cada sección se borra igual que si el profesor la
        // hubiera borrado a mano.
        foreach (var section in sectionRepository.GetByCourseId(course.Id))
            deleteSectionUseCase.Execute(new DeleteSectionCommand(section.Id));

        enrollmentRepository.DeleteByCourseId(course.Id);
        cohortRepository.DeleteByCourseId(course.Id);
        courseRepository.Delete(course.Id);
    }
}
