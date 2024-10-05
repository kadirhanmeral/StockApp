using App.Application.Features.Buy;
using App.Application.Features.Stocks;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(IStockService stockService) : ControllerBase
{
    
    [HttpGet("GetStockList")]
    public async Task<IActionResult> GetStockList()
    {
        var stocks = await stockService.GetStockListAsync();
        return Ok(stocks);
    }
    
    [HttpPost("PushBist100DataToDatabase")]
    public async Task<IActionResult> PushBist100DataToDatabase()
    {
        var result = await stockService.PushBist100DataToDatabase();
        return Ok(result);
    }
    
    [HttpPost("PushStockDetailsToDatabase")]
    public async Task<IActionResult> PushStockDetailsToDatabase()
    {
        var result = await stockService.PushStockDetailsToDatabase();
        return Ok(result);
    }
    [HttpPut("UpdateStockDetails")]
    public async Task<IActionResult> UpdateStockDetails()
    {
        var result = await stockService.UpdateStockDetails();
        return Ok(result);
    }
    [HttpGet("GetStocksDataPaged")]
    public async Task<IActionResult> GetStocksDataPaged(int page , int pageSize )
    {
        var result = await stockService.GetStocksDataPaged(page,pageSize);
        return Ok(result);
    }
    [HttpGet("GetStockDetails")]
    public async Task<IActionResult> GetStockDetails(string symbol)
    {
        var result = await stockService.GetStockDetails(symbol);
        return Ok(result);
    }
    [HttpPost("BuyStock")]
    public async Task<IActionResult> BuyStock(BuyStockRequest buyStock)
    {
        var result = await stockService.BuyStock(buyStock);
        return Ok(result);
    }
    [HttpPost("SellStock")]
    public async Task<IActionResult> SellStock(BuyStockRequest buyStock)
    {
        var result = await stockService.SellStock(buyStock);
        return Ok(result);
    }
    
    
    
}