using System.ComponentModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DigitalWallet.Data.Models;
using DigitalWallet.Services;

namespace DigitalWallet.Pages;

public class TransactionDetailsModel(
    WalletManager walletManager,
    TransactionManager transactionManager)
    : PageModel
{
    public class ClientData
    {
        public required string Email { get; set; }

        [DisplayName("Wallet Id")]
        public Guid? WalletId { get; set; }
    }

    public Transaction Transaction { get; set; } = default!;

    public ClientData SenderData { get; set; } = default!;

    public ClientData ReceiverData { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var transaction = await transactionManager.FindByIdAsync(id.Value);
        if (transaction == null)
        {
            return NotFound();
        }

        Transaction = transaction;
        SenderData = await GetTransactionClientNameAsync(t => t.SenderId);
        ReceiverData = await GetTransactionClientNameAsync(t => t.ReceiverId);
        return Page();
    }

    private async Task<ClientData> GetTransactionClientNameAsync(Func<Transaction, Guid?> clientIdFunc)
    {
        var clientId = clientIdFunc(Transaction);
        if (clientId is null)
        {
            return new ClientData { Email = Transaction.ExternalCustomer };
        }

        var wallet = await walletManager.FindByIdAsync(clientId.Value);
        var client = await walletManager.GetClientAsync(wallet);
        return new ClientData { Email = client.Email, WalletId = wallet.Id };
    }
}
