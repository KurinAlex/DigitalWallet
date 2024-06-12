using System.ComponentModel.DataAnnotations;

using Stripe.Checkout;

namespace DigitalWallet.Data.Models;

public class Transaction : IEntity
{
    private Session? _stripeSession;
    private readonly SessionService _stripeSessionService = new();

    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public DateTimeOffset Start { get; set; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public DateTimeOffset? End { get; set; }

    public TransactionStatus Status { get; set; }

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

    public string SenderName => StripeSession?.CustomerDetails.Email ?? Sender?.Client?.Email ?? "Wallet " + SenderId;

    public string ReceiverName => Company?.Name ?? Receiver?.Client?.Email ?? "Wallet " + ReceiverId;

    public bool IsInternalSender => SenderId is not null;

    public bool IsExternalSender => StripeSessionId is not null;

    public bool IsInternalReceiver => ReceiverId is not null;

    public bool IsCompanyReceiver => CompanyId is not null;

    public bool IsSenderClientDataDeleted => Sender?.Client is null;

    public bool IsReceiverClientDataDeleted => Receiver?.Client is null;

    public TransactionType TransactionType
    {
        get
        {
            if (IsExternalSender && IsInternalReceiver)
            {
                return TransactionType.Deposit;
            }

            if (IsInternalSender && IsInternalReceiver)
            {
                return TransactionType.Transaction;
            }

            if (IsInternalSender && IsCompanyReceiver)
            {
                return TransactionType.Payment;
            }

            return TransactionType.Unknown;
        }
    }
}
