namespace Studia.Domain.Sections;

public class Section
{
    public Guid Id { get; }
    public Guid CourseId { get; }
    public string Title { get; }
    public string DescriptionHtml { get; }

    private Section(Guid id, Guid courseId, string title, string descriptionHtml)
    {
        Id = id;
        CourseId = courseId;
        Title = title;
        DescriptionHtml = descriptionHtml;
    }

    public static Section Create(Guid courseId, string title, string descriptionHtml)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("El curso no es válido.", nameof(courseId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la sección no puede estar vacío.", nameof(title));

        if (title.Length > 150)
            throw new ArgumentException("El título de la sección no puede superar los 150 caracteres.", nameof(title));

        return new Section(Guid.NewGuid(), courseId, title.Trim(), descriptionHtml.Trim());
    }
}
