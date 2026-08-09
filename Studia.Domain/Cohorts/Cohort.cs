namespace Studia.Domain.Cohorts;

public class Cohort
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public string Name { get; }

    private readonly List<Guid> _studentIds = [];
    public IReadOnlyCollection<Guid> StudentIds => _studentIds.AsReadOnly();

    private Cohort(Guid id, Guid courseId, string name)
    {
        Id = id;
        CourseId = courseId;
        Name = name;
    }

    public static Cohort Create(Guid courseId, string name)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("La ficha debe pertenecer a un curso.", nameof(courseId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la ficha no puede estar vacío.", nameof(name));

        return new Cohort(Guid.NewGuid(), courseId, name.Trim());
    }

    public void AssignStudent(Guid studentId)
    {
        if (_studentIds.Contains(studentId))
            throw new InvalidOperationException("El estudiante ya está asignado a esta ficha.");

        _studentIds.Add(studentId);
    }
}
