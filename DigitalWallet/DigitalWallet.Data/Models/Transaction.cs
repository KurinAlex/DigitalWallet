using System.ComponentModel.DataAnnotations;

using Stripe.Checkout;

namespace DigitalWallet.Data.Models;

public class Transaction : IEntity
{
    private Session? _stripeSession;
    private readonly SessionService _stripeSessionService = new();

    public Guid Id { get; set; }

    public required decimal Amount { get; init; }

    [StringLength(100)]
    public string? Description { get; set; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public required DateTimeOffset Start { get; init; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public DateTimeOffset? End { get; set; }

    public required TransactionStatus Status { get; set; }

    public required TransactionType Type { get; init; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public Guid? CompanyId { get; set; }

    [StringLength(80)]
    public string? StripeSessionId { get; set; }

    public Wallet? Sender { get; set; }

    public Wallet? Receiver { get; set; }

    public Session? StripeSession => StripeSessionId is null
        ? null :
        _stripeSession ??= _stripeSessionService.Get(StripeSessionId);

    public Company? Company { get; set; }

    public string? SenderName => Type switch
    {
        TransactionType.Deposit => StripeSession?.CustomerDetails.Name ?? "External Client",
        TransactionType.Transfer or TransactionType.Payment => Sender?.Client?.UserName ?? "Wallet " + SenderId,
        _ => null
    };

    public string? ReceiverName => Type switch
    {
        TransactionType.Deposit or TransactionType.Transfer => Receiver?.Client?.UserName ?? "Wallet " + ReceiverId,
        TransactionType.Payment => Company?.Name ?? "Company",
        _ => null
    };
}
