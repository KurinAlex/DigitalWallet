using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services.Managers;

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
            .Include(s => s.Company)
            .SingleOrDefaultAsync(w => w.Id == id);
    }

    public Task<List<Transaction>> GetTransactionsAsync(Wallet wallet)
    {
        return _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.SenderId == wallet.Id || t.ReceiverId == wallet.Id)
            .Include(t => t.Sender!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Receiver!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Company)
            .ToListAsync();
    }

    public async Task<Transaction> StartTransactionAsync(
        decimal amount,
        string? description = null,
        Guid? senderId = null,
        Guid? receiverId = null,
        string? stripeSessionId = null,
        Guid? companyId = null)
    {
        var transaction = new Transaction
        {
            Amount = amount,
            Description = description,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress,
            SenderId = senderId,
            ReceiverId = receiverId,
            StripeSessionId = stripeSessionId,
            CompanyId = companyId,
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

    private Task SetStatusAndFinishAsync(Transaction transaction, TransactionStatus status)
    {
        transaction.Status = status;
        transaction.End = DateTimeOffset.Now;
        return UpdateAsync(transaction);
    }
}
