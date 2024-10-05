using App.Application.Features.Currencies.Buy;
using App.Application.Features.Currencies.Dto;
using App.Domain.Entities;

namespace App.Application.Features.Currencies;

public interface ICurrencyService
{
    Task<ServiceResult<CurrencyListDto>> GetCurrencyListAsync();
    Task<ServiceResult> PushCurrenciesToDatabase();
    Task<ServiceResult> UpdateCurrenciesInDatabaseAsync();
    Task<ServiceResult<List<Currency>>> GetCurrenciesDataPaged(int page, int pageSize);

    Task<ServiceResult<CurrencyDto>> GetCurrencyDetails(string symbol);
    Task<ServiceResult> BuyCurrency(BuyCurrencyRequest sellCurrency);
    Task<ServiceResult> SellCurrency(BuyCurrencyRequest sellCurrency);
}