using DigitalWallet.Data;
using DigitalWallet.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services.Managers;

public class Manager<T>(ApplicationDbContext dbContext)
    where T : class, IEntity
{
    public Task<T?> FindByIdAsync(Guid? id)
    {
        return dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task CreateAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(T entity)
    {
        dbContext.Set<T>().Update(entity);
        return dbContext.SaveChangesAsync();
    }

    public Task UpdateRangeAsync(params T[] entities)
    {
        dbContext.Set<T>().UpdateRange(entities);
        return dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        return dbContext.SaveChangesAsync();
    }
}
