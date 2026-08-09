namespace Studia.Domain.Activities;

public class Activity
{
    public Guid Id { get; }
    public Guid SectionId { get; }
    public string Title { get; }
    public string Description { get; }
    public DateTime DueDateUtc { get; }
    public ActivityType Type { get; }
    public int? MaxFiles { get; }

    private Activity(Guid id, Guid sectionId, string title, string description, DateTime dueDateUtc, ActivityType type, int? maxFiles)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        Description = description;
        DueDateUtc = dueDateUtc;
        Type = type;
        MaxFiles = maxFiles;
    }

    public static Activity Create(Guid sectionId, string title, string description, DateTime dueDateUtc, ActivityType type, int? maxFiles)
    {
        if (sectionId == Guid.Empty)
            throw new ArgumentException("La sección no es válida.", nameof(sectionId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la actividad no puede estar vacío.", nameof(title));

        if (title.Length > 150)
            throw new ArgumentException("El título de la actividad no puede superar los 150 caracteres.", nameof(title));

        switch (type)
        {
            case ActivityType.ConArchivo when maxFiles is null or <= 0:
                throw new ArgumentException("Una actividad con archivo debe indicar un máximo de archivos mayor a cero.", nameof(maxFiles));
            case ActivityType.SoloTexto when maxFiles is not null:
                throw new ArgumentException("Una actividad de solo texto no debe tener un máximo de archivos.", nameof(maxFiles));
        }

        return new Activity(Guid.NewGuid(), sectionId, title.Trim(), description?.Trim() ?? string.Empty, dueDateUtc, type, maxFiles);
    }
}
