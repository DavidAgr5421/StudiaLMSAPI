namespace Studia.Application.Activities;

public interface IGetActivityFileUseCase
{
    ActivityFileContentResult Execute(GetActivityFileQuery query);
}
