using Microsoft.EntityFrameworkCore;
using Studia.Application.Auth;
using Studia.Domain.Auth;

namespace Studia.Infrastructure.Persistence.EfCore.Repositories;

public class EfRevokedTokenRepository(StudiaDbContext dbContext) : IRevokedTokenRepository
{
    public void Save(RevokedToken revokedToken)
    {
        if (dbContext.RevokedTokens.Any(r => r.Jti == revokedToken.Jti))
            dbContext.RevokedTokens.Update(revokedToken);
        else
            dbContext.RevokedTokens.Add(revokedToken);

        dbContext.SaveChanges();
    }

    public RevokedToken? GetByJti(string jti) => dbContext.RevokedTokens.FirstOrDefault(r => r.Jti == jti);
}
