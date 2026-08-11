using Studia.Application.Cohorts;
using Studia.Application.Users;
using Studia.Domain.Courses;

namespace Studia.Application.Courses;

public class SearchCoursesUseCase(
    ICourseRepository courseRepository,
    ICohortRepository cohortRepository,
    IUserRepository userRepository) : ISearchCoursesUseCase
{
    public IReadOnlyCollection<CourseResult> Execute(SearchCoursesQuery query)
    {
        var trimmed = query.Query.Trim();

        var matched = string.IsNullOrEmpty(trimmed)
            // Sin término: "todos los cursos disponibles" (RF12, botón "Cursos" público) --
            // Search("") ya trae todo por cómo funciona Contains, pero acá lo hacemos explícito.
            ? courseRepository.Search("")
            : MatchByTerm(trimmed);

        // Solo los activos son "visibles al público": un curso archivado sigue existiendo
        // (entregas, historial) pero no tiene sentido que aparezca para buscarlo o unirse.
        return matched
            .Where(course => course.Status == CourseStatus.Activo)
            .DistinctBy(course => course.Id)
            .Select(WithProfesorName)
            .ToList();
    }

    private IEnumerable<Course> MatchByTerm(string term)
    {
        var matchedByName = courseRepository.Search(term);

        var matchedByCohort = cohortRepository.Search(term)
            .Select(cohort => courseRepository.GetById(cohort.CourseId))
            .Where(course => course is not null)
            .Select(course => course!);

        // El profesor no está en el nombre del curso ni de la ficha -- para "buscar por
        // nombre o email del profesor" hay que resolver primero qué usuarios matchean y
        // después traer los cursos que ese usuario dicta.
        var matchedByProfesor = userRepository.Search(term)
            .SelectMany(user => courseRepository.GetByProfesorId(user.Id));

        return matchedByName.Concat(matchedByCohort).Concat(matchedByProfesor);
    }

    private CourseResult WithProfesorName(Course course)
    {
        var profesor = userRepository.GetById(course.ProfesorId);
        return CourseResult.FromDomain(course) with { ProfesorName = profesor?.Name };
    }
}
