using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Data;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(255);
            entity.Property(c => c.CustomerNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(c => c.CustomerNumber).IsUnique();
            entity.HasIndex(c => c.Email).IsUnique();
        });

        // Configure Account entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(a => a.AccountType).IsRequired().HasMaxLength(50);
            entity.Property(a => a.AccountName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Balance).HasPrecision(18, 2);
            entity.Property(a => a.AvailableBalance).HasPrecision(18, 2);
            entity.Property(a => a.Currency).HasMaxLength(3);
            entity.HasIndex(a => a.AccountNumber).IsUnique();
            entity.HasIndex(a => a.CustomerId);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.TransactionType).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Category).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.BalanceAfter).HasPrecision(18, 2);
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.Merchant).HasMaxLength(200);
            entity.Property(t => t.Status).HasMaxLength(20);
            entity.Property(t => t.ReferenceNumber).HasMaxLength(50);
            entity.HasIndex(t => t.AccountId);
            entity.HasIndex(t => t.TransactionDate);
            entity.HasIndex(t => t.ReferenceNumber);
        });
    }
}
