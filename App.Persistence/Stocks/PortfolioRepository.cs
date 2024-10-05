using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence.Stocks;

public class PortfolioRepository(StockDbContext context):GenericRepository<Portfolio,int>(context),IPortfolioRepository
{
    public Task<List<Portfolio>> GetPortfolioByUserIdAsync(int userId)
    {
        return context.Portfolios.Where(x => x.Id == userId).ToListAsync();
    }
    
    public Task<Portfolio?> GetPortfolioByUserIdAndSymbolAsync(int userId,string symbol)
    {
        return context.Portfolios.Where(x => x.Id == userId && x.StockSymbol == symbol).FirstOrDefaultAsync();

    }
}