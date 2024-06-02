using DigitalWallet.Models;
using DigitalWallet.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace DigitalWallet.Pages
{
    public class TransactionsModel(
        UserManager<Client> clientManager,
        WalletManager walletManager,
        TransactionManager transactionManager)
        : PageModel
    {
        public async Task<IActionResult> OnGetTransactionsAsync()
        {
            var client = await clientManager.GetUserAsync(User);
            var wallet = await walletManager.FindByClientAsync(client);
            return new JsonResult(await transactionManager.GetTransactionsAsync(wallet));
        }
    }
}
