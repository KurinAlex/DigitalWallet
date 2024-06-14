namespace DigitalWallet.Services.Models;

public class CompanyViewModel
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required long WalletsCount { get; set; }

    public required long TransactionsCount { get; set; }
}
