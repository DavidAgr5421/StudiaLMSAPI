namespace Studia.Domain.Sections;

public class Section
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public string Title { get; }
    public string DescriptionHtml { get; }

    // Oculto: solo la ve el profesor dueño del curso (o un admin) -- no aparece para
    // estudiantes ni dispara notificaciones. Pensado para preparar contenido antes de
    // publicarlo.
    public SectionStatus Status { get; }

    // Vacío = global (visible para todo el curso). Si tiene elementos, solo los
    // estudiantes de esas fichas pueden ver la sección.
    private readonly List<Guid> _cohortIds = [];
    public IReadOnlyCollection<Guid> CohortIds => _cohortIds.AsReadOnly();

    private Section(Guid id, Guid courseId, string title, string descriptionHtml, SectionStatus status)
    {
        Id = id;
        CourseId = courseId;
        Title = title;
        DescriptionHtml = descriptionHtml;
        Status = status;
    }

    public static Section Create(
        Guid courseId,
        string title,
        string descriptionHtml,
        IReadOnlyCollection<Guid>? cohortIds = null,
        SectionStatus status = SectionStatus.Visible)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("El curso no es válido.", nameof(courseId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la sección no puede estar vacío.", nameof(title));

        if (title.Length > 150)
            throw new ArgumentException("El título de la sección no puede superar los 150 caracteres.", nameof(title));

        var section = new Section(Guid.NewGuid(), courseId, title.Trim(), descriptionHtml.Trim(), status);
        section._cohortIds.AddRange(cohortIds ?? []);

        return section;
    }
}
