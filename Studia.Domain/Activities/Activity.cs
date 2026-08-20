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

    // Individual/Grupal ya funcionan; Foro/Evaluación son a futuro (ver ActivityKind).
    public ActivityKind Kind { get; }

    // Antes de esta fecha la actividad no es visible para estudiantes (se comporta como
    // Oculto), aunque el profesor/admin ya puede verla y prepararla. Null = sin fecha de
    // apertura, siempre visible desde que se crea (sujeto igual al Status).
    public DateTime? OpenDateUtc { get; }

    // Decidido por el profesor al crear la actividad: si es false, una vez pasada la
    // fecha límite ya no se puede entregar (ni editar) en absoluto, en vez de aceptarse
    // marcada como tardía.
    public bool AllowsLateSubmission { get; }

    // Cierre manual: el profesor puede bloquear la actividad en cualquier momento, sin
    // importar la fecha límite ni AllowsLateSubmission.
    public DateTime? ManuallyClosedAtUtc { get; private set; }

    // Oculto: solo la ve el profesor dueño del curso (o un admin) -- no aparece para
    // estudiantes ni dispara notificaciones. Pensado para preparar contenido antes de
    // publicarlo.
    public ActivityStatus Status { get; }

    // Vacío = global (visible para todo el curso). Si tiene elementos, solo los
    // estudiantes de esas fichas pueden ver la actividad. Para Kind = Grupal, estas
    // mismas fichas son los grupos: cada una entrega una sola vez, en conjunto.
    private readonly List<Guid> _cohortIds = [];
    public IReadOnlyCollection<Guid> CohortIds => _cohortIds.AsReadOnly();

    // Material de apoyo que sube el profesor -- distinto de las entregas de los estudiantes.
    private readonly List<ActivityFile> _files = [];
    public IReadOnlyCollection<ActivityFile> Files => _files.AsReadOnly();

    private Activity(
        Guid id,
        Guid sectionId,
        string title,
        string description,
        DateTime dueDateUtc,
        ActivityType type,
        int? maxFiles,
        ActivityStatus status,
        ActivityKind kind,
        DateTime? openDateUtc,
        bool allowsLateSubmission)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        Description = description;
        DueDateUtc = dueDateUtc;
        Type = type;
        MaxFiles = maxFiles;
        Status = status;
        Kind = kind;
        OpenDateUtc = openDateUtc;
        AllowsLateSubmission = allowsLateSubmission;
    }

    public static Activity Create(
        Guid sectionId,
        string title,
        string description,
        DateTime dueDateUtc,
        ActivityType type,
        int? maxFiles,
        IReadOnlyCollection<Guid>? cohortIds = null,
        IReadOnlyCollection<ActivityFile>? files = null,
        ActivityStatus status = ActivityStatus.Visible,
        ActivityKind kind = ActivityKind.Individual,
        DateTime? openDateUtc = null,
        bool allowsLateSubmission = true)
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

        if (openDateUtc is not null && openDateUtc > dueDateUtc)
            throw new ArgumentException("La fecha de apertura no puede ser posterior a la fecha límite.", nameof(openDateUtc));

        var resolvedCohortIds = cohortIds ?? [];
        if (kind == ActivityKind.Grupal && resolvedCohortIds.Count == 0)
            throw new ArgumentException("Una actividad grupal necesita al menos una ficha que haga de grupo.", nameof(cohortIds));

        var activity = new Activity(
            Guid.NewGuid(), sectionId, title.Trim(), description?.Trim() ?? string.Empty, dueDateUtc, type, maxFiles, status, kind, openDateUtc, allowsLateSubmission);
        activity._cohortIds.AddRange(resolvedCohortIds);
        activity._files.AddRange(files ?? []);

        return activity;
    }

    // true si a esta hora ya llegó (o no tiene) la fecha de apertura -- lo que decide si
    // un estudiante puede verla. El profesor/admin no pasa por acá, siempre la ve.
    public bool HasOpenedAt(DateTime nowUtc) => OpenDateUtc is null || nowUtc >= OpenDateUtc;

    // true si a esta hora todavía se puede entregar (o editar una entrega existente).
    public bool AcceptsSubmissionsAt(DateTime nowUtc)
    {
        if (ManuallyClosedAtUtc is not null)
            return false;

        return nowUtc <= DueDateUtc || AllowsLateSubmission;
    }

    public void Close()
    {
        if (ManuallyClosedAtUtc is not null)
            throw new InvalidOperationException("La actividad ya está cerrada.");

        ManuallyClosedAtUtc = DateTime.UtcNow;
    }

    public void Reopen()
    {
        if (ManuallyClosedAtUtc is null)
            throw new InvalidOperationException("La actividad no está cerrada.");

        ManuallyClosedAtUtc = null;
    }
}
