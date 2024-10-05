using App.Application.Features.Buy;
using App.Application.Features.Dto;
using App.Application.Features.Stocks.Dto;
using App.Domain.Entities;

namespace App.Application.Features.Stocks;

public interface IStockService
{
    Task<ServiceResult<StockListDto>> GetStockListAsync();
    Task<ServiceResult> PushBist100DataToDatabase();
    Task<ServiceResult> PushStockDetailsToDatabase();
    Task<ServiceResult> UpdateStockDetails();
    Task<ServiceResult<List<StockDetails>>> GetStocksDataPaged(int page, int pageSize);

    Task<ServiceResult<StockDetails>> GetStockDetails(string symbol);

    Task<ServiceResult> BuyStock(BuyStockRequest buyStock);
    Task<ServiceResult> SellStock(BuyStockRequest sellStock);
}