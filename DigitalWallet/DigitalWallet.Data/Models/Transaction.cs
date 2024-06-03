namespace DigitalWallet.Data.Models;

public class Transaction : IEntity
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public TransactionStatus Status { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public string? ExternalCustomer { get; set; }
}
