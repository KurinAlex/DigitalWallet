using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using DigitalWallet.Services.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services;

public class TransactionManager(ApplicationDbContext dbContext) : Manager<Transaction>(dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Transaction?> FindByIdWithDetails(Guid id)
    {
        return _dbContext.Transactions
            .Include(t => t.Sender!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Receiver!)
            .ThenInclude(w => w.Client)
            .SingleOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IList<Transaction>> GetTransactionsAsync(Wallet wallet)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.SenderId == wallet.Id || t.ReceiverId == wallet.Id)
            .Include(t => t.Sender!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Receiver!)
            .ThenInclude(w => w.Client)
            .ToListAsync();
    }

    public async Task<Transaction> StartTransactionAsync(
        decimal amount,
        Guid? senderId = null,
        Guid? receiverId = null,
        string? externalCustomer = null)
    {
        var transaction = new Transaction
        {
            ReceiverId = receiverId,
            SenderId = senderId,
            ExternalCustomer = externalCustomer,
            Amount = amount,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress
        };

        await CreateAsync(transaction);
        return transaction;
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
