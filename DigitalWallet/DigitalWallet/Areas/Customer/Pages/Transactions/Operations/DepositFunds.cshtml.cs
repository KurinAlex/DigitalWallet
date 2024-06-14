using System.ComponentModel.DataAnnotations;

using DigitalWallet.Data.Models;
using DigitalWallet.Helpers;
using DigitalWallet.Services.Managers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Stripe.Checkout;

namespace DigitalWallet.Areas.Customer.Pages.Transactions.Operations;

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
        if (client is null)
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
        var transaction = await transactionManager.FindByStripeSessionId(sessionId);
        if (transaction is not null)
        {
            return BadRequest("Transaction is already completed.");
        }

        var service = new SessionService();
        var session = await service.GetAsync(sessionId);
        if (session?.AmountTotal is null)
        {
            return ActionResultHelper.GetEntityNotFoundResult("Session");
        }

        if (session.PaymentStatus != "paid")
        {
            return BadRequest("Transaction is not paid");
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

        transaction = new Transaction
        {
            Amount = session.AmountTotal.Value / 100m,
            Description = DepositTransactionDescription,
            ReceiverId = wallet.Id,
            StripeSessionId = session.Id,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress,
            Type = TransactionType.Deposit
        };

        await transactionManager.CreateAsync(transaction);

        try
        {
            await walletManager.DepositAsync(wallet, transaction.Amount);
            await transactionManager.SetSucceededAndFinishAsync(transaction);
        }
        catch
        {
            await transactionManager.SetFailedAndFinishAsync(transaction);
        }

        return RedirectToPage("/Transactions/TransactionDetails", new { id = transaction.Id });
    }
}
