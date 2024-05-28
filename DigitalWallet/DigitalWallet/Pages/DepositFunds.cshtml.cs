using System.ComponentModel.DataAnnotations;

using DigitalWallet.Models;
using DigitalWallet.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class DepositFundsModel(UserManager<Client> userManager, WalletManager walletManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(decimal), "0,01", "1000000")]
        public decimal Amount { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = await userManager.GetUserAsync(User);
        if (client == null)
        {
            return Forbid();
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return Forbid();
        }

        var result = await walletManager.DepositAsync(wallet, Input.Amount);
        if (result.Succeeded)
        {
            return LocalRedirect(Url.Content("~/"));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty,error);
        }

        return Page();
    }
}
