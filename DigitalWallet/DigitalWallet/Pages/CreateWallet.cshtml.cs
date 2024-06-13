using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class CreateWalletModel(UserManager<Client> userManager, WalletManager walletManager) : PageModel
{
    public async Task<IActionResult> OnPost()
    {
        var client = await userManager.GetUserAsync(User);
        if (client is null)
        {
            return ActionResultHelper.GetClientNotFoundResult();
        }

        var wallet = new Wallet
        {
            ClientId = client.Id
        };

        await walletManager.CreateAsync(wallet);
        return RedirectToPage("/WalletDetails", new { id = wallet.Id });
    }
}
