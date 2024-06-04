using System.ComponentModel.DataAnnotations;

namespace DigitalWallet.Data.Models;

public class Transaction : IEntity
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public DateTimeOffset Start { get; set; }

    [DisplayFormat(DataFormatString = "{0:O}")]
    public DateTimeOffset? End { get; set; }

    public TransactionStatus Status { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    [MaxLength(100)]
    public string? ExternalCustomer { get; set; }

    public Wallet? Sender { get; set; }

    public Wallet? Receiver { get; set; }

    public string? SenderName => Sender is null ? ExternalCustomer : Sender.Client?.ToString();

    public string? ReceiverName => Receiver is null ? ExternalCustomer : Receiver.Client?.ToString();
}
