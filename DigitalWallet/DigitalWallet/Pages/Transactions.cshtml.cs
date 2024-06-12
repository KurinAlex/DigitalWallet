using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Pages;

public class TransactionsModel(
    UserManager<Client> clientManager,
    WalletManager walletManager,
    TransactionManager transactionManager)
    : PageModel
{
    private record TransactionViewModel(
        Guid Id,
        decimal Amount,
        string Type,
        string Subject,
        string Status,
        DateTimeOffset Time);

    public async Task<IActionResult> OnGetAllTransactionsAsync()
    {
        var client = await clientManager.GetUserAsync(User);
        if (client is null)
        {
            return ActionResultHelper.GetClientNotFoundResult();
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return ActionResultHelper.GetClientDoesNotHaveWalletResult();
        }

        var transactions = await transactionManager.GetTransactionsAsync(wallet);
        var transactionsVm = transactions.Select(t => new TransactionViewModel(
            t.Id,
            Type: t.TransactionType.ToString(),
            Amount: t.SenderId == wallet.Id ? -t.Amount : t.Amount,
            Subject: t.SenderId == wallet.Id ? t.ReceiverName : t.SenderName,
            Status: t.Status.ToString(),
            Time: t.End ?? t.Start
        )).ToList();

        return new JsonResult(transactionsVm);
    }
}
