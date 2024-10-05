using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence.Stocks;

public class TransactionHistoriesRepository(StockDbContext context):GenericRepository<TransactionHistory,int>(context),ITransactionHistoriesRepository
{
    public Task<List<TransactionHistory>> GetTransactionsByUserIdAsync(int userId)
    {
        var result = context.TransactionHistories.Where(x => x.UserId == userId).OrderByDescending(x=>x.Time).ToListAsync();
        return result;
    }
}