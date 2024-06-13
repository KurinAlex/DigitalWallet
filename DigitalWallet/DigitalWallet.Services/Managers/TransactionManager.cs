using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using DigitalWallet.Services.Models;

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

    public Task<List<TransactionViewModel>> GetTransactionsAsync(Wallet wallet)
    {
        return _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.SenderId == wallet.Id || t.ReceiverId == wallet.Id)
            .Include(t => t.Sender!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Receiver!)
            .ThenInclude(w => w.Client)
            .Include(t => t.Company)
            .Select(t => new TransactionViewModel
            {
                Id = t.Id,
                Amount = t.SenderId == wallet.Id ? -t.Amount : t.Amount,
                Subject = t.SenderId == wallet.Id ? t.ReceiverName : t.SenderName,
                Status = t.Status.ToString(),
                Type = t.Type.ToString(),
                Time = t.End ?? t.Start
            })
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

    private Task SetStatusAndFinishAsync(Transaction transaction, TransactionStatus status)
    {
        transaction.Status = status;
        transaction.End = DateTimeOffset.Now;
        return UpdateAsync(transaction);
    }
}
