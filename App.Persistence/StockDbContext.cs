using System.Reflection;
using App.Domain.Entities;
using App.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace App.Persistence;

public class StockDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public StockDbContext(DbContextOptions<StockDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; } = default!;
    public DbSet<StockDetails> StocksDetails { get; set; } = default!;
    public DbSet<BlackListToken> BlackListedTokens { get; set; } = default!;
    public DbSet<TransactionHistory> TransactionHistories { get; set; } = default!;
    public DbSet<Portfolio> Portfolios { get; set; } = default!;
    
    public DbSet<Currency> Currencies { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
