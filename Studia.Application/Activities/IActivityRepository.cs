using Studia.Domain.Activities;

namespace Studia.Application.Activities;

public interface IActivityRepository
{
    void Save(Activity activity);

    Activity? GetById(Guid id);

    IReadOnlyCollection<Activity> GetBySectionId(Guid sectionId);

    // Usado por el recordatorio automático de fecha límite -- evita traer toda la tabla
    // para filtrar en memoria.
    IReadOnlyCollection<Activity> GetWithDueDateBetween(DateTime fromUtc, DateTime toUtc);

    void DeleteBySectionId(Guid sectionId);
}
