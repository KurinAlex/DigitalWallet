namespace DigitalWallet.Data.Models;

public class Wallet : IEntity
{
    public Guid Id { get; set; }

    public decimal Balance { get; set; }

    public Guid ClientId { get; set; }

    public Client Client { get; set; } = default!;
}
