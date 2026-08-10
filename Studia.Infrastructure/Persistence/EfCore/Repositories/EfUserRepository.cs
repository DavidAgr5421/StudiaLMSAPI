using Microsoft.EntityFrameworkCore;
using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfUserRepository(StudiaDbContext dbContext) : IUserRepository
{
    public void Save(User user)
    {
        if (dbContext.Users.Any(u => u.Id == user.Id))
            dbContext.Users.Update(user);
        else
            dbContext.Users.Add(user);

        dbContext.SaveChanges();
    }

    public User? GetById(Guid id) => dbContext.Users.FirstOrDefault(u => u.Id == id);

    public User? GetByEmail(Email email) => dbContext.Users.FirstOrDefault(u => u.Email == email);

    // Búsqueda difusa sobre Email.Value: EF no puede traducir el acceso a esa propiedad
    // del Value Object a SQL, así que se filtra en memoria (aceptable a esta escala).
    public IReadOnlyCollection<User> Search(string query) =>
        dbContext.Users
            .AsEnumerable()
            .Where(u => (u.Name is not null && u.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                || u.Email.Value.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
}
