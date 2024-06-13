using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Data.Models;

[Index(nameof(Name), IsUnique = true)]
public class Company : IEntity
{
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = default!;

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [Url]
    public string? Site { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
