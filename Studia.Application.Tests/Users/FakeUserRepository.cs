using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Application.Tests.Users;

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();

    public IReadOnlyCollection<User> SavedUsers => _users.Values.ToList();

    public void Save(User user) => _users[user.Id] = user;

    public User? GetById(Guid id) => _users.GetValueOrDefault(id);

    public User? GetByEmail(Email email) => _users.Values.FirstOrDefault(u => u.Email.Equals(email));
}
