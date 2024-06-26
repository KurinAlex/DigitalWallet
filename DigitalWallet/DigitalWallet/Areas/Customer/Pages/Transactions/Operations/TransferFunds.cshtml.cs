using System.ComponentModel.DataAnnotations;

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Areas.Customer.Pages.Transactions.Operations;

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

        [StringLength(100)]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var senderClient = await userManager.GetUserAsync(User);
        if (senderClient is null)
        {
            return ActionResultHelper.GetClientNotFoundResult();
        }

        var senderWallet = await walletManager.FindByClientAsync(senderClient);
        if (senderWallet is null)
        {
            return ActionResultHelper.GetClientDoesNotHaveWalletResult();
        }

        if (senderWallet.Balance < Input.Amount)
        {
            ModelState.AddModelError("Input.Amount", "You don't have enough funds.");
            return Page();
        }

        Data.Models.Wallet? receiverWallet = default;
        var receiverClient = await userManager.FindByEmailAsync(Input.Receiver);

        if (receiverClient is not null)
        {
            receiverWallet = await walletManager.FindByClientAsync(receiverClient);
        }
        else if (Guid.TryParse(Input.Receiver, out var walletToId))
        {
            receiverWallet = await walletManager.FindByIdAsync(walletToId);
        }

        if (receiverWallet is null)
        {
            var error = receiverClient is null ? "Client or wallet not found." : "This client don't have wallet yet.";
            ModelState.AddModelError("Input.Receiver", error);
            return Page();
        }

        if (receiverWallet == senderWallet)
        {
            ModelState.AddModelError("Input.Receiver", "You cannot transfer to yourself :)");
            return Page();
        }

        var transaction = new Transaction
        {
            Amount = Input.Amount,
            Description = Input.Description,
            SenderId = senderWallet.Id,
            ReceiverId = receiverWallet.Id,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress,
            Type = TransactionType.Transfer
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

        return RedirectToPage("/Transactions/TransactionDetails", new { id = transaction.Id });
    }
}
