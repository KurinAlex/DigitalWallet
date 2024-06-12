using DigitalWallet.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<Client, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Client>()
            .HasOne(c => c.Wallet)
            .WithOne(w => w.Client)
            .HasForeignKey<Wallet>(w => w.ClientId);

        builder.Entity<Wallet>()
            .HasMany<Transaction>()
            .WithOne(w => w.Sender)
            .HasForeignKey(t => t.SenderId);

        builder.Entity<Wallet>()
            .HasMany<Transaction>()
            .WithOne(w => w.Receiver)
            .HasForeignKey(t => t.ReceiverId);

        builder.Entity<Company>()
            .HasMany<Transaction>()
            .WithOne(w => w.Company)
            .HasForeignKey(t => t.CompanyId);
    }
}
