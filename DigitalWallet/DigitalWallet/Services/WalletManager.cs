using DigitalWallet.Data;
using DigitalWallet.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services;

public class WalletManager(ApplicationDbContext dbContext) : Manager<Wallet>(dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Wallet?> FindByClientAsync(Client client)
    {
        return _dbContext.Wallets.SingleOrDefaultAsync(w => w.ClientId == client.Id);
    }

    public Task<Client?> GetClientAsync(Wallet wallet)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Id == wallet.ClientId);
    }

    public Task DepositAsync(Wallet wallet, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        wallet.Balance += amount;
        return UpdateAsync(wallet);
    }

    public Task WithdrawAsync(Wallet wallet, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        ArgumentOutOfRangeException.ThrowIfLessThan(wallet.Balance, amount);

        wallet.Balance -= amount;
        return UpdateAsync(wallet);
    }

    public Task TransferAsync(Wallet senderWallet, Wallet receiverWallet, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        ArgumentOutOfRangeException.ThrowIfLessThan(senderWallet.Balance, amount);

        senderWallet.Balance -= amount;
        receiverWallet.Balance += amount;
        return UpdateRangeAsync(senderWallet, receiverWallet);
    }
}
