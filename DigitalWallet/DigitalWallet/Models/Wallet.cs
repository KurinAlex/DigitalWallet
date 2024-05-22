namespace DigitalWallet.Models;

public class Wallet
{
    public Guid Id { get; set; }

    public decimal Balance { get; set; }

    public Guid ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
