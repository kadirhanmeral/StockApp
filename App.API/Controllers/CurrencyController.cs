using App.Application.Features.Buy;
using App.Application.Features.Currencies;
using App.Application.Features.Currencies.Buy;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(ICurrencyService currencyService) : ControllerBase
{
    [HttpGet("GetCurrencyList")]
    public async Task<IActionResult> GetStockList()
    {
        var result = await currencyService.GetCurrencyListAsync();
        return Ok(result);
    }
    [HttpPost("PushCurrenciesToDatabase")]
    public async Task<IActionResult> PushCurrenciesToDatabase()
    {
        var result = await currencyService.PushCurrenciesToDatabase();
        return Ok(result);
    }
    
    [HttpPatch("UpdateCurrenciesInDatabase")]
    public async Task<IActionResult> UpdateCurrenciesInDatabaseAsync()
    {
        var result = await currencyService.UpdateCurrenciesInDatabaseAsync();
        return Ok(result);
    }
    [HttpGet("GetCurrenciesDataPaged")]
    public async Task<IActionResult> GetCurrenciesDataPaged(int page , int pageSize )
    {
        var result = await currencyService.GetCurrenciesDataPaged(page,pageSize);
        return Ok(result);
    }
    
    [HttpGet("GetCurrencyDetails")]
    public async Task<IActionResult> GetCurrencyDetails(string name)
    {
        var result = await currencyService.GetCurrencyDetails(name);
        return Ok(result);
    }
    
    [HttpPost("BuyCurrency")]
    public async Task<IActionResult> BuyCurrency(BuyCurrencyRequest request)
    {
        var result = await currencyService.BuyCurrency(request);
        return Ok(result);
    }
    
    [HttpPost("SellCurrency")]
    public async Task<IActionResult> SellCurrency(BuyCurrencyRequest request)
    {
        var result = await currencyService.SellCurrency(request);
        return Ok(result);
    }
}