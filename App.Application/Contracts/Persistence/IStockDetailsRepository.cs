using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface IStockDetailsRepository:IGenericRepository<StockDetails,int>
{
    Task<decimal> GetCurrentPriceAsync(string symbol);
}