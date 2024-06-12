using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class WalletDetailsModel(UserManager<Client> userManager, WalletManager walletManager) : PageModel
{
    public Wallet Wallet { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var client = await userManager.GetUserAsync(User);
        if (client == null)
        {
            return ActionResultHelper.GetClientNotFoundResult();
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return ActionResultHelper.GetClientDoesNotHaveWalletResult();
        }

        Wallet = wallet;
        return Page();
    }
}
