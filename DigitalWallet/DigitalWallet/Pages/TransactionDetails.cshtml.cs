using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services;

namespace DigitalWallet.Pages;

public class TransactionDetailsModel(TransactionManager transactionManager) : PageModel
{
    public Transaction Transaction { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var transaction = await transactionManager.FindByIdWithDetails(id.Value);
        if (transaction == null)
        {
            return ActionResultHelper.GetEntityNotFoundResult("Transaction");
        }

        Transaction = transaction;
        return Page();
    }
}
