﻿using DigitalWallet.Data;
using DigitalWallet.Models;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Services;

public class WalletManager(ApplicationDbContext dbContext)
{
    public void SetClient(Wallet wallet, Guid clientId)
    {
        wallet.ClientId = clientId;
    }

    public async Task<Wallet?> FindByIdAsync(Guid id)
    {
        return await dbContext.Wallets.FindAsync(id);
    }

    public async Task<Wallet?> FindByClientAsync(Client client)
    {
        await dbContext.Entry(client).Reference(c => c.Wallet).LoadAsync();
        return client.Wallet;
    }

    public async Task<OperationResult> CreateAsync(Wallet wallet)
    {
        await dbContext.Wallets.AddAsync(wallet);
        return await TrySaveChangesAsync();
    }

    public Task<OperationResult> DepositAsync(Wallet wallet, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        wallet.Balance += amount;
        return TrySaveChangesAsync();
    }

    public async Task<OperationResult> WithdrawAsync(Wallet wallet, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (amount > wallet.Balance)
        {
            return OperationResult.Failed("Wallet don't have enough funds.");
        }

        wallet.Balance -= amount;
        return await TrySaveChangesAsync();
    }

    public async Task<OperationResult> TransferAsync(Wallet from, Wallet to, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (amount > from.Balance)
        {
            return OperationResult.Failed("Sender's wallet don't have enough funds.");
        }

        from.Balance -= amount;
        to.Balance += amount;
        return await TrySaveChangesAsync();
    }

    public Task<OperationResult> DeleteAsync(Wallet wallet)
    {
        dbContext.Wallets.Remove(wallet);
        return TrySaveChangesAsync();
    }

    private async Task<OperationResult> TrySaveChangesAsync()
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return OperationResult.Success;
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }
}
