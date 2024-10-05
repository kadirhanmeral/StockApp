using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    IStockDetailsRepository StocksDetails { get; }
    ITransactionHistoriesRepository TransactionHistories { get; }
    IPortfolioRepository Portfolios { get; }
    
    IGenericRepository<BlackListToken, int> BlackListedTokens { get; }
    
    IGenericRepository<Currency, int> Currencies { get; }

    IGenericRepository<Stock, int> StockRepository { get; }

    Task<int> SaveChangesAsync();
}

