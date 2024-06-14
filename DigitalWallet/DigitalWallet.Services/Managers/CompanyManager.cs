using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using DigitalWallet.Services.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services.Managers;

public class CompanyManager(ApplicationDbContext dbContext) : Manager<Company>(dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Company?> FindByNameAsync(string companyName)
    {
        return _dbContext.Companies.AsNoTracking().SingleOrDefaultAsync(c => c.Name == companyName);
    }

    public Task<List<CompanyViewModel>> GetAllAsync(bool sorted = false)
    {
        var query = _dbContext.Companies.Select(c => new CompanyViewModel
        {
            Id = c.Id,
            Name = c.Name,
            WalletsCount = c.Transactions.Select(t => t.SenderId).Distinct().Count(),
            TransactionsCount = c.Transactions.Count
        });

        if (sorted)
        {
            query = query.OrderBy(c => c.Name);
        }

        return query.ToListAsync();
    }

    public Task RemoveCompanyFromTransactionsAsync(Company company)
    {
        return _dbContext.Entry(company)
            .Collection(c => c.Transactions)
            .Query()
            .ExecuteUpdateAsync(p => p.SetProperty(t => t.CompanyId, (Guid?)null));
    }
}
