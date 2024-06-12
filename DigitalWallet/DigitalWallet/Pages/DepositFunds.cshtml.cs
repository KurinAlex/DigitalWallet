using System.ComponentModel.DataAnnotations;

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Stripe.Checkout;

namespace DigitalWallet.Pages;

public class DepositFundsModel(
    UserManager<Client> userManager,
    WalletManager walletManager,
    TransactionManager transactionManager)
    : PageModel
{
    private const string DepositTransactionDescription = "Deposit to Wallet";

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        [Required]
        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal Amount { get; set; }
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

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes =
            [
                "card"
            ],
            Mode = "payment",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = Convert.ToInt64(decimal.Round(Input.Amount, 2) * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = DepositTransactionDescription
                        }
                    },
                    Quantity = 1
                }
            ],
            SuccessUrl = Url.PageLink(pageHandler: "ProceedPayment") + "&sessionId={CHECKOUT_SESSION_ID}",
            CancelUrl = Url.PageLink()
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return Redirect(session.Url);
    }

    public async Task<IActionResult> OnGetProceedPaymentAsync(string sessionId)
    {
        var service = new SessionService();
        var session = await service.GetAsync(sessionId);
        if (session?.AmountTotal is null)
        {
            return ActionResultHelper.GetEntityNotFoundResult("Session");
        }

        var client = await userManager.GetUserAsync(User);
        if (client is null)
        {
            return ActionResultHelper.GetClientNotFoundResult();
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return ActionResultHelper.GetClientDoesNotHaveWalletResult();
        }

        var transaction = await transactionManager.StartTransactionAsync(
            amount: session.AmountTotal.Value / 100m,
            description: DepositTransactionDescription,
            receiverId: wallet.Id,
            stripeSessionId: session.Id);

        try
        {
            await walletManager.DepositAsync(wallet, transaction.Amount);
            await transactionManager.SetSucceededAndFinishAsync(transaction);
        }
        catch
        {
            await transactionManager.SetFailedAndFinishAsync(transaction);
        }

        return RedirectToPage("TransactionDetails", new { id = transaction.Id });
    }
}
