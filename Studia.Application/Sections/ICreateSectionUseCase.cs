namespace Studia.Application.Sections;

public interface ICreateSectionUseCase
{
    SectionResult Execute(CreateSectionCommand command);
}
