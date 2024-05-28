using System.ComponentModel.DataAnnotations;

using DigitalWallet.Models;
using DigitalWallet.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class TransferFundsModel(UserManager<Client> userManager, WalletManager walletManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Required]
        public string Receiver { get; set; } = default!;

        [Required]
        [Range(typeof(decimal), "0,01", "1000000")]
        public decimal Amount { get; set; }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var clientFrom = await userManager.GetUserAsync(User);
        if (clientFrom == null)
        {
            return Forbid();
        }

        var walletFrom = await walletManager.FindByClientAsync(clientFrom);
        if (walletFrom is null)
        {
            return Forbid();
        }

        Wallet? walletTo = default;
        var clientTo = await userManager.FindByEmailAsync(Input.Receiver);

        if (clientTo != null)
        {
            walletTo = await walletManager.FindByClientAsync(clientTo);
        }
        else if (Guid.TryParse(Input.Receiver, out var walletToId))
        {
            walletTo = await walletManager.FindByIdAsync(walletToId);
        }

        if (walletTo is null)
        {
            var error = clientTo is null ? "Client or wallet not found." : "This client don't have wallet yet.";
            ModelState.AddModelError("Input.Receiver", error);
            return Page();
        }

        if (walletTo == walletFrom)
        {
            ModelState.AddModelError(string.Empty, "You cannot transfer to yourself :)");
            return Page();
        }

        var result = await walletManager.TransferAsync(walletFrom, walletTo, Input.Amount);
        if (result.Succeeded)
        {
            return LocalRedirect(Url.Content("~/"));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return Page();
    }
}
