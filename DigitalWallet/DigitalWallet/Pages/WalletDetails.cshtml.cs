using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DigitalWallet.Models;
using DigitalWallet.Services;

namespace DigitalWallet.Pages
{
    public class WalletDetailsModel(UserManager<Client> userManager, WalletManager walletManager) : PageModel
    {
        public Wallet Wallet { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var client = await userManager.GetUserAsync(User);
            if (client == null)
            {
                return Forbid();
            }
            
            var wallet = await walletManager.FindByClientAsync(client);
            if (wallet is not null)
            {
                Wallet = wallet;
                return Page();
            }

            wallet = new Wallet();
            walletManager.SetClient(wallet, client.Id);
            var result = await walletManager.CreateAsync(wallet);
            if (result.Succeeded)
            {
                return Page();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }
    }
}
