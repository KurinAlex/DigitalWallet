using DigitalWallet.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<Client, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Client>()
            .HasOne<Wallet>()
            .WithOne()
            .HasForeignKey<Wallet>(w => w.ClientId);

        builder.Entity<Wallet>()
            .HasMany<Transaction>()
            .WithOne()
            .HasForeignKey(t => t.SenderId)
            .HasForeignKey(t => t.ReceiverId);
    }
}
