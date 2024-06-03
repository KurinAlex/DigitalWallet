using DigitalWallet.Data;
using DigitalWallet.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services;

public class TransactionManager(ApplicationDbContext dbContext) : Manager<Transaction>(dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IList<Transaction>> GetTransactionsAsync(Wallet wallet)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.SenderId == wallet.Id || t.ReceiverId == wallet.Id)
            .ToListAsync();
    }

    public Task SetSucceededAndFinishAsync(Transaction transaction)
    {
        return SetStatusAndFinishAsync(transaction, TransactionStatus.Succeeded);
    }

    public Task SetFailedAndFinishAsync(Transaction transaction)
    {
        return SetStatusAndFinishAsync(transaction, TransactionStatus.Failed);
    }


    public Task SetStatusAndFinishAsync(Transaction transaction, TransactionStatus status)
    {
        transaction.Status = status;
        transaction.End = DateTimeOffset.Now;
        return UpdateAsync(transaction);
    }
}
