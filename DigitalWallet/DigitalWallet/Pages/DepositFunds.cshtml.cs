using System.ComponentModel.DataAnnotations;

using DigitalWallet.Models;
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
            return NotFound("Can't identify user.");
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return BadRequest("You can't deposit funds without wallet.");
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
                            Name = "Deposit to Wallet",
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
            return NotFound("Session with specified ID not found.");
        }

        var client = await userManager.GetUserAsync(User);
        if (client is null)
        {
            return NotFound("User not found.");
        }

        var wallet = await walletManager.FindByClientAsync(client);
        if (wallet is null)
        {
            return BadRequest();
        }

        var transaction = new Transaction
        {
            ReceiverId = wallet.Id,
            ExternalCustomer = session.CustomerDetails.Email,
            Amount = session.AmountTotal.Value / 100m,
            Start = DateTimeOffset.Now,
            Status = TransactionStatus.InProgress
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

        return RedirectToPage("TransactionDetails", new { id = transaction.Id });
    }
}
