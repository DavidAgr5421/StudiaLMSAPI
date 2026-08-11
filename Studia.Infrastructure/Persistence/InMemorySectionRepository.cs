using System.Collections.Concurrent;
using Studia.Application.Sections;
using Studia.Domain.Sections;

namespace Studia.Infrastructure.Persistence;

public class InMemorySectionRepository : ISectionRepository
{
    private readonly ConcurrentDictionary<Guid, Section> _sections = new();

    public void Save(Section section) => _sections[section.Id] = section;

    public Section? GetById(Guid id) => _sections.GetValueOrDefault(id);

    public IReadOnlyCollection<Section> GetByCourseId(Guid courseId) =>
        _sections.Values.Where(s => s.CourseId == courseId).ToList();

    public void Delete(Guid id) => _sections.TryRemove(id, out _);
}
