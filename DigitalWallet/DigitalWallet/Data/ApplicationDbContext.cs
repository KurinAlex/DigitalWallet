using DigitalWallet.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<Client, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Wallet> Wallets { get; set; }
}
