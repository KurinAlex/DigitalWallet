namespace DigitalWallet.Services.Models;

public class TransactionViewModel
{
    public required Guid Id { get; set; }

    public required decimal Amount { get; set; }

    public required string Type { get; set; }

    public required string Subject { get; set; }

    public required string Status { get; set; }

    public required DateTimeOffset Time { get; set; }
}
