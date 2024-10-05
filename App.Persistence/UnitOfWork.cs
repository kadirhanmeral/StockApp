using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using App.Persistence.Stocks;

namespace App.Persistence
{
    public sealed class UnitOfWork(StockDbContext context) : IUnitOfWork, IDisposable
    {
        private IGenericRepository<Stock, int>? _stockRepository;
        private IStockDetailsRepository? _stocksDetails;
        private IGenericRepository<BlackListToken, int>? _blackListedTokens;
        private IGenericRepository<Currency, int>? _currencies;
        private ITransactionHistoriesRepository? _transactionHistories;
        private IPortfolioRepository? _portfolios;

        public IGenericRepository<Stock, int> StockRepository => 
            _stockRepository ??= new GenericRepository<Stock, int>(context);

        public IStockDetailsRepository StocksDetails => 
            _stocksDetails ??= new StockDetailsRepository(context);

        public IGenericRepository<BlackListToken, int> BlackListedTokens => 
            _blackListedTokens ??= new GenericRepository<BlackListToken, int>(context);

        public IGenericRepository<Currency, int> Currencies => 
            _currencies ??= new GenericRepository<Currency, int>(context);

        public ITransactionHistoriesRepository TransactionHistories => 
            _transactionHistories ??= new TransactionHistoriesRepository(context);

        public IPortfolioRepository Portfolios => 
            _portfolios ??= new PortfolioRepository(context);

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
    }
}
