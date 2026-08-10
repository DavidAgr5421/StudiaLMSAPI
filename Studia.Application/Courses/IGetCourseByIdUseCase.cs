namespace Studia.Application.Courses;

public interface IGetCourseByIdUseCase
{
    // Nullable a propósito: "no existe" es un resultado válido de una consulta, no un
    // error -- lo distinto de los comandos, donde el mismo caso se modela como excepción.
    CourseResult? Execute(GetCourseByIdQuery query);
}
