using Studia.Application.Submissions;

namespace Studia.Application.Activities;

public class GetActivityFileUseCase(IActivityRepository activityRepository, IFileStorage fileStorage) : IGetActivityFileUseCase
{
    public ActivityFileContentResult Execute(GetActivityFileQuery query)
    {
        var activity = activityRepository.GetById(query.ActivityId)
            ?? throw new InvalidOperationException($"No existe una actividad con id '{query.ActivityId}'.");

        // Ata el storageKey a ESTA actividad -- sin este chequeo, cualquier storageKey
        // válido de otra actividad serviría igual, filtrando material de otro curso.
        var file = activity.Files.FirstOrDefault(f => f.StorageKey == query.StorageKey)
            ?? throw new InvalidOperationException("El archivo no pertenece a esta actividad.");

        var content = fileStorage.Retrieve(file.StorageKey)
            ?? throw new InvalidOperationException("No se pudo encontrar el contenido del archivo.");

        return new ActivityFileContentResult(file.FileName, content);
    }
}
