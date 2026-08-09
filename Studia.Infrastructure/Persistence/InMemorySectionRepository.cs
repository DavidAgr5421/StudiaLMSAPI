using System.Collections.Concurrent;
using Studia.Application.Sections;
using Studia.Domain.Sections;

namespace Studia.Infrastructure.Persistence;

public class InMemorySectionRepository : ISectionRepository
{
    private readonly ConcurrentDictionary<Guid, Section> _sections = new();

    public void Save(Section section) => _sections[section.Id] = section;

    public Section? GetById(Guid id) => _sections.GetValueOrDefault(id);
}
