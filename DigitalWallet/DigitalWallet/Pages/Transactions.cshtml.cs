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
        return new JsonResult(transactions);
    }
}
