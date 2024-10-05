using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface ITransactionHistoriesRepository:IGenericRepository<TransactionHistory,int>
{
    Task<List<TransactionHistory>> GetTransactionsByUserIdAsync(int userId);
    
}