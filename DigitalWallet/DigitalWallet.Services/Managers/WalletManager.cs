using DigitalWallet.Data;
using DigitalWallet.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services.Managers;

public class WalletManager(ApplicationDbContext dbContext) : Manager<Wallet>(dbContext)
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Wallet?> FindByClientAsync(Client client)
    {
        await _dbContext.Entry(client).Reference(c => c.Wallet).LoadAsync();
        return client.Wallet;
    }

    public Task RemoveWalletFromClientAsync(Client client)
    {
        return _dbContext.Entry(client)
            .Reference(c => c.Wallet)
            .Query()
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.ClientId, (Guid?)null));
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
