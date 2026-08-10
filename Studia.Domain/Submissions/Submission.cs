namespace Studia.Domain.Submissions;

public class Submission
{
    public Guid Id { get; }
    public Guid ActivityId { get; }
    public Guid StudentId { get; }
    public SubmissionStatus Status { get; }
    public DateTime SubmittedAtUtc { get; }
    public string? TextContent { get; }
    public int? Score { get; private set; }
    public string? Feedback { get; private set; }

    private readonly List<SubmittedFile> _files;
    public IReadOnlyCollection<SubmittedFile> Files => _files.AsReadOnly();

    // Constructor vacío exclusivo para que EF Core materialice el objeto al leerlo de la
    // base de datos (no puede emparejar el parámetro "files" del otro constructor con el
    // campo "_files": difieren en nombre y tipo). El dominio nunca llama a este constructor.
#pragma warning disable CS8618
    private Submission()
    {
        _files = [];
    }
#pragma warning restore CS8618

    private Submission(
        Guid id,
        Guid activityId,
        Guid studentId,
        SubmissionStatus status,
        DateTime submittedAtUtc,
        string? textContent,
        IReadOnlyCollection<SubmittedFile> files)
    {
        Id = id;
        ActivityId = activityId;
        StudentId = studentId;
        Status = status;
        SubmittedAtUtc = submittedAtUtc;
        TextContent = textContent;
        _files = files.ToList();
    }

    public static Submission SubmitText(Guid activityId, Guid studentId, string textContent, DateTime dueDateUtc)
    {
        ValidateIds(activityId, studentId);

        if (string.IsNullOrWhiteSpace(textContent))
            throw new ArgumentException("El contenido de la entrega no puede estar vacío.", nameof(textContent));

        var submittedAtUtc = DateTime.UtcNow;
        var status = submittedAtUtc > dueDateUtc ? SubmissionStatus.Tardia : SubmissionStatus.ATiempo;

        return new Submission(Guid.NewGuid(), activityId, studentId, status, submittedAtUtc, textContent.Trim(), []);
    }

    public static Submission SubmitWithFiles(Guid activityId, Guid studentId, IReadOnlyCollection<SubmittedFile> files, int maxFiles, DateTime dueDateUtc)
    {
        ValidateIds(activityId, studentId);

        if (files.Count == 0)
            throw new ArgumentException("Debe adjuntar al menos un archivo.", nameof(files));

        if (files.Count > maxFiles)
            throw new ArgumentException($"La entrega supera el máximo de {maxFiles} archivo(s) permitidos.", nameof(files));

        var submittedAtUtc = DateTime.UtcNow;
        var status = submittedAtUtc > dueDateUtc ? SubmissionStatus.Tardia : SubmissionStatus.ATiempo;

        return new Submission(Guid.NewGuid(), activityId, studentId, status, submittedAtUtc, null, files.ToList());
    }

    public void Grade(int score, string? feedback)
    {
        if (score is < 0 or > 100)
            throw new ArgumentException("La calificación debe estar entre 0 y 100.", nameof(score));

        Score = score;
        Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
    }

    private static void ValidateIds(Guid activityId, Guid studentId)
    {
        if (activityId == Guid.Empty)
            throw new ArgumentException("La actividad no es válida.", nameof(activityId));

        if (studentId == Guid.Empty)
            throw new ArgumentException("El estudiante no es válido.", nameof(studentId));
    }
}
