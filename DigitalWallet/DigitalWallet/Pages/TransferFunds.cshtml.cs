using System.ComponentModel.DataAnnotations;

using DigitalWallet.Data.Models;
using DigitalWallet.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Pages;

public class TransferFundsModel(
    UserManager<Client> userManager,
    WalletManager walletManager,
    TransactionManager transactionManager)
    : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Required]
        public string Receiver { get; set; } = default!;

        [Required]
        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal Amount { get; set; }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var clientFrom = await userManager.GetUserAsync(User);
        if (clientFrom == null)
        {
            return NotFound();
        }

        var senderWallet = await walletManager.FindByClientAsync(clientFrom);
        if (senderWallet is null)
        {
            return BadRequest();
        }

        Wallet? receiverWallet = default;
        var clientTo = await userManager.FindByEmailAsync(Input.Receiver);

        if (clientTo != null)
        {
            receiverWallet = await walletManager.FindByClientAsync(clientTo);
        }
        else if (Guid.TryParse(Input.Receiver, out var walletToId))
        {
            receiverWallet = await walletManager.FindByIdAsync(walletToId);
        }

        if (receiverWallet is null)
        {
            var error = clientTo is null ? "Client or wallet not found." : "This client don't have wallet yet.";
            ModelState.AddModelError("Input.Receiver", error);
            return Page();
        }

        if (receiverWallet == senderWallet)
        {
            ModelState.AddModelError(string.Empty, "You cannot transfer to yourself :)");
            return Page();
        }

        if (senderWallet.Balance < Input.Amount)
        {
            ModelState.AddModelError("Input.Amount", "You don't have enough funds.");
        }

        var transaction = new Transaction
        {
            SenderId = senderWallet.Id,
            ReceiverId = receiverWallet.Id,
            Amount = Input.Amount,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress
        };

        await transactionManager.CreateAsync(transaction);

        try
        {
            await walletManager.TransferAsync(senderWallet, receiverWallet, Input.Amount);
            await transactionManager.SetSucceededAndFinishAsync(transaction);
        }
        catch
        {
            await transactionManager.SetFailedAndFinishAsync(transaction);
        }

        return RedirectToPage("TransactionDetails", new { id = transaction.Id });
    }
}
