using System.Collections.Concurrent;
using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Infrastructure.Persistence;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public void Save(User user) => _users[user.Id] = user;

    public User? GetById(Guid id) => _users.GetValueOrDefault(id);

    public User? GetByEmail(Email email) =>
        _users.Values.FirstOrDefault(u => u.Email.Equals(email));

    public IReadOnlyCollection<User> Search(string query) =>
        _users.Values
            .Where(u => (u.Name is not null && u.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                || u.Email.Value.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
}
