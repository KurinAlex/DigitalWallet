using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace DigitalWallet.Pages;

public class MakePaymentModel(
    CompanyManager companyManager,
    UserManager<Client> userManager,
    WalletManager walletManager,
    TransactionManager transactionManager) : PageModel
{
    public SelectList Companies { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [DisplayName("Company")]
        [Required(ErrorMessage = "The Company field is required")]
        public Guid? CompanyId { get; set; }

        [Required]
        [StringLength(100)]
        public string Service { get; set; } = default!;

        [Required]
        public decimal Amount { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateCompanies();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
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

        if (wallet.Balance < Input.Amount)
        {
            ModelState.AddModelError("Input.Amount", "You don't have enough funds.");
            await PopulateCompanies(Input.CompanyId);
            return Page();
        }

        var transaction = await transactionManager.StartTransactionAsync(
            amount: Input.Amount,
            description: Input.Service,
            senderId: wallet.Id,
            companyId: Input.CompanyId);

        try
        {
            await walletManager.WithdrawAsync(wallet, transaction.Amount);
            await transactionManager.SetSucceededAndFinishAsync(transaction);
        }
        catch
        {
            await transactionManager.SetFailedAndFinishAsync(transaction);
        }

        return RedirectToPage("TransactionDetails", new { id = transaction.Id });
    }

    private async Task PopulateCompanies(Guid? selectedId = null)
    {
        var companies = await companyManager.GetAll();
        Companies = new SelectList(companies, nameof(Company.Id), nameof(Company.Name), selectedId);
    }
}
