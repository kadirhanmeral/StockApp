using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence.Stocks;

public class StockDetailsRepository(StockDbContext context):GenericRepository<StockDetails,int>(context),IStockDetailsRepository
{
    public Task<decimal> GetCurrentPriceAsync(string symbol)
    {
         
        var result = context.StocksDetails.Where(x => x.Symbol == symbol)
            .Select(x => x.Price).FirstOrDefaultAsync();
        return result;
    }
}
