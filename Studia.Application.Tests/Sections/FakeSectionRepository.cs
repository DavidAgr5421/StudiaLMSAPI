using Studia.Application.Sections;
using Studia.Domain.Sections;

namespace Studia.Application.Tests.Sections;

public class FakeSectionRepository : ISectionRepository
{
    private readonly Dictionary<Guid, Section> _sections = new();

    public IReadOnlyCollection<Section> SavedSections => _sections.Values.ToList();

    public void Save(Section section) => _sections[section.Id] = section;

    public Section? GetById(Guid id) => _sections.GetValueOrDefault(id);
}
