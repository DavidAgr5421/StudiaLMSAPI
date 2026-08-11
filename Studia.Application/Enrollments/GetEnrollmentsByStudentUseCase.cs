namespace Studia.Application.Enrollments;

// "Mis inscripciones": a diferencia de GetEnrollmentsByCourseUseCase (que arma la vista del
// profesor para aprobar/rechazar), acá el propio estudiante es el dueño de los datos -- no
// hace falta completar StudentName/StudentEmail porque ya los conoce de su propia sesión.
public class GetEnrollmentsByStudentUseCase(IEnrollmentRepository enrollmentRepository) : IGetEnrollmentsByStudentUseCase
{
    public IReadOnlyCollection<EnrollmentResult> Execute(GetEnrollmentsByStudentQuery query) =>
        enrollmentRepository.GetByStudentId(query.StudentId)
            .Select(EnrollmentResult.FromDomain)
            .ToList();
}
