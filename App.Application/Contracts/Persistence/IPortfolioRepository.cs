using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface IPortfolioRepository:IGenericRepository<Portfolio,int>
{
    Task<List<Portfolio>> GetPortfolioByUserIdAsync(int userId);
    Task<Portfolio?> GetPortfolioByUserIdAndSymbolAsync(int userId, string symbol);
}