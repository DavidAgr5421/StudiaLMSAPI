using Studia.Domain.Users;

namespace Studia.Application.Users;

public interface IUserRepository
{
    void Save(User user);

    User? GetById(Guid id);

    User? GetByEmail(Email email);
}
