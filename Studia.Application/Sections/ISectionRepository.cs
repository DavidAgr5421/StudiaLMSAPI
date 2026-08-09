using Studia.Domain.Sections;

namespace Studia.Application.Sections;

public interface ISectionRepository
{
    void Save(Section section);

    Section? GetById(Guid id);
}
