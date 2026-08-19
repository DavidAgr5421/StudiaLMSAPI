using Microsoft.EntityFrameworkCore;
using Studia.Application.Auth;
using Studia.Domain.Auth;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfPasswordResetTokenRepository(StudiaDbContext dbContext) : IPasswordResetTokenRepository
{
    public void Save(PasswordResetToken token)
    {
        if (dbContext.PasswordResetTokens.Any(t => t.Id == token.Id))
            dbContext.PasswordResetTokens.Update(token);
        else
            dbContext.PasswordResetTokens.Add(token);

        dbContext.SaveChanges();
    }

    public PasswordResetToken? GetByTokenHash(string tokenHash) =>
        dbContext.PasswordResetTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
}
