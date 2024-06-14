using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalWallet.Areas.Customer.Pages.Transactions;

public class TransactionDetailsModel(
    TransactionManager transactionManager,
    UserManager<Client> clientManager,
    WalletManager walletManager)
    : PageModel
{
    public Transaction Transaction { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var transaction = await transactionManager.FindByIdWithDetails(id.Value);
        if (transaction is null)
        {
            return ActionResultHelper.GetEntityNotFoundResult("Transfer");
        }

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

        if (!await clientManager.IsInRoleAsync(client, StaticData.AdminRoleName)
            && wallet.Id != transaction.SenderId
            && wallet.Id != transaction.ReceiverId)
        {
            return Forbid("You don't have permission to view this page");
        }

        Transaction = transaction;
        return Page();
    }
}
