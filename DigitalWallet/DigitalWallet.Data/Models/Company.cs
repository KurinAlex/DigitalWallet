
using System.ComponentModel.DataAnnotations;

namespace DigitalWallet.Data.Models;

public class Company : IEntity
{
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = default!;
}
