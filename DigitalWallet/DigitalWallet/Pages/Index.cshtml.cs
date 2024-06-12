using DigitalWallet.Data.Models;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class IndexModel(WalletManager walletManager, UserManager<Client> userManager) : PageModel
{
    public Wallet? Wallet { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = await userManager.GetUserAsync(User);
        if (client is not null)
        {
            Wallet = await walletManager.FindByClientAsync(client);
        }

        return Page();
    }
}
