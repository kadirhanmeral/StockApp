using System.Globalization;
using System.Net;
using System.Security.Claims;
using App.Application.Contracts.Authorization;
using App.Application.Contracts.Persistence;
using App.Application.Features.Buy;
using App.Application.Features.Currencies.Buy;
using App.Application.Features.Currencies.Dto;
using App.Domain.Entities;
using Newtonsoft.Json;

namespace App.Application.Features.Currencies;

public class CurrencyService(IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService,
    HttpClient httpClient) : ICurrencyService
{
    
    public async Task<ServiceResult<CurrencyListDto>> GetCurrencyListAsync()
    {
        var response = await httpClient.GetStringAsync("https://finans.truncgil.com/today.json");

        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

        var exchangeRates = new CurrencyListDto
        {
            Data = []
        };

        if (jsonObject != null && jsonObject.TryGetValue("Update_Date", out var updateDateValue))
        {
            exchangeRates.UpdateDate = DateTime.ParseExact(updateDateValue.ToString() ?? string.Empty, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        foreach (var rate in from keyValue in jsonObject where keyValue.Key != "Update_Date" let rateData = JsonConvert.DeserializeObject<Dictionary<string, string>>(keyValue.Value.ToString()) select new CurrencyDto
                 {
                     Currency = keyValue.Key,
                     Type = rateData["Tür"],
                     BuyRate = decimal.Parse((string)rateData["Alış"].Replace("$", ""), new CultureInfo("tr-TR")), 
                     SellRate = decimal.Parse((string)rateData["Satış"].Replace("$", ""), new CultureInfo("tr-TR")), 
                     Change = decimal.Parse((string)rateData["Değişim"].Replace("%", "")),
                     CurrencyCode = rateData["Alış"].StartsWith($"$") ? "USD" : "TRY" 
                 })
        {
            exchangeRates.Data.Add(rate);
        }

        return  ServiceResult<CurrencyListDto>.Success(exchangeRates);
    }
    
    public async Task<ServiceResult> PushCurrenciesToDatabase()
    {

        var currencyList = await GetCurrencyListAsync();

        foreach (var currency in currencyList.Data!.Data)
        {
            
                var newCurrency = new Currency
                {
                    Name = currency.Currency,
                    Type = currency.Type,
                    Buy = currency.BuyRate,
                    Sell = currency.SellRate,
                    ChangePercent = currency.Change,
                    CurrencyCode = currency.CurrencyCode,
                    Created = currencyList.Data.UpdateDate
                    
                };
                await unitOfWork.Currencies.AddAsync(newCurrency);
            
        }

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
  
    }
    
    public async Task<ServiceResult> UpdateCurrenciesInDatabaseAsync()
    {
        var currencyList = await GetCurrencyListAsync();

        foreach (var currency in currencyList.Data!.Data)
        {
            var existingCurrency = await unitOfWork.Currencies.FirstOrDefaultAsync(c=>c.Name == currency.Currency);
         
            if (existingCurrency == null)
            {
                return ServiceResult.Fail("Currency not found", HttpStatusCode.NotFound);
            }
            existingCurrency.Buy = currency.BuyRate;
            existingCurrency.Sell = currency.SellRate;
            existingCurrency.ChangePercent = currency.Change;
            existingCurrency.Updated = currencyList.Data.UpdateDate;
            unitOfWork.Currencies.Update(existingCurrency); 
        }

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }
    
    
    public async Task<ServiceResult<List<Currency>>> GetCurrenciesDataPaged(int page, int pageSize)
    {

        var currencies = await unitOfWork.Currencies.GetAllPagedAsync(page, pageSize);
        

        return ServiceResult<List<Currency>>.Success(currencies);
        
    }
    
    public async Task<ServiceResult<CurrencyDto>> GetCurrencyDetails(string symbol)
    {

        
        var currency = await unitOfWork.Currencies.FirstOrDefaultAsync(s => s.Name == symbol);

        if (currency ==null)
        {
            return ServiceResult<CurrencyDto>.Fail("Currency not found",HttpStatusCode.NotFound);
        }
            
        var result = new CurrencyDto()
        {
            Currency = currency.Name,
            Type = currency.Type,
            BuyRate = currency.Buy,
            SellRate = currency.Sell,
            Change = currency.ChangePercent,
            CurrencyCode = currency.CurrencyCode,
                
        };

        return ServiceResult<CurrencyDto>.Success(result);

    }
    public async Task<ServiceResult> BuyCurrency(BuyCurrencyRequest buyCurrency)
    {
   
        var result =await authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);
        if (result.IsFail)
        {
            return ServiceResult.Fail(result.ErrorMessages!);
        }

        var existingCurrency = await unitOfWork.Currencies.FirstOrDefaultAsync(s => s.Name == buyCurrency.Symbol);
        if (existingCurrency == null)
        {
            return ServiceResult.Fail("Currency not found", HttpStatusCode.NotFound);
        }
        var transaction = new TransactionHistory
        {
            UserId = int.Parse(result.Data!),
            StockSymbol = buyCurrency.Symbol,
            Time = DateTime.Now,
            Price = buyCurrency.Price,
            Quantity = buyCurrency.Quantity,
            TradeType = TradeType.Buy

        };
        await unitOfWork.TransactionHistories.AddAsync(transaction);
        
        var existingPortfolio = await unitOfWork.Portfolios.FirstOrDefaultAsync(x =>
            x.StockSymbol == buyCurrency.Symbol && x.Id == int.Parse(result.Data!));
        
        
        if (existingPortfolio != null)
        {
            decimal totalPriceOfNewTransaction;
            if (existingPortfolio.StockSymbol == "ons")
            {
                var usdPrice = await unitOfWork.Currencies.FirstOrDefaultAsync(x => x.Name == "usd");
                totalPriceOfNewTransaction = buyCurrency.Quantity * buyCurrency.Price*usdPrice!.Buy;

            }
            else
            {
                totalPriceOfNewTransaction = buyCurrency.Quantity * buyCurrency.Price;

            }
            var totalPriceOfPortfolio = existingPortfolio.BuyPrice * existingPortfolio.Quantity;
            existingPortfolio.Quantity += buyCurrency.Quantity;
            existingPortfolio.BuyPrice = (totalPriceOfPortfolio + totalPriceOfNewTransaction) /
                                         (existingPortfolio.Quantity);

        }
        else
        {
            Portfolio newStock;
            if (buyCurrency.Symbol == "ons")
            {
                var usdPrice = await unitOfWork.Currencies.FirstOrDefaultAsync(x => x.Name == "usd");
                newStock = new Portfolio
                {
                    Id = int.Parse(result.Data!),
                    StockSymbol = buyCurrency.Symbol,
                    BuyPrice = buyCurrency.Price * usdPrice!.Buy,
                    Quantity = buyCurrency.Quantity,
                    Type = existingCurrency.Type
                };
            }
            else
            { 
                newStock = new Portfolio
                {
                    Id = int.Parse(result.Data!),
                    StockSymbol = buyCurrency.Symbol,
                    BuyPrice = buyCurrency.Price,
                    Quantity = buyCurrency.Quantity,
                    Type = existingCurrency.Type
                };
            }

            await unitOfWork.Portfolios.AddAsync(newStock);
        }
        
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    

    }
    
    public async Task<ServiceResult> SellCurrency(BuyCurrencyRequest sellCurrency)
    {
   
        var result =await authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);
        var claim = authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);

        if (result.IsFail)
        {
            return ServiceResult.Fail(result.ErrorMessages!);
        }

        var existingCurrency = await unitOfWork.Currencies.FirstOrDefaultAsync(s => s.Name == sellCurrency.Symbol);
        if (existingCurrency == null)
        {
            return ServiceResult.Fail("Currency not found", HttpStatusCode.NotFound);
        }

        var userId = int.Parse(claim.Result.Data!);

        var dataList = await unitOfWork.Portfolios.GetPortfolioByUserIdAndSymbolAsync(userId, sellCurrency.Symbol);

        if (dataList==null)
        {
            return ServiceResult.Fail("You don't have any currency available to sell");

        }

        if (sellCurrency.Quantity>dataList.Quantity)
        {
            return ServiceResult.Fail("You can't sell more than you have");

        }
        
        

        var transaction = new TransactionHistory
        {
            UserId = int.Parse(result.Data!),
            StockSymbol = sellCurrency.Symbol,
            Time = DateTime.Now,
            Price = sellCurrency.Price,
            Quantity = sellCurrency.Quantity,
            TradeType = TradeType.Sell

        };
        await unitOfWork.TransactionHistories.AddAsync(transaction);
        
        var existingPortfolio = await unitOfWork.Portfolios.FirstOrDefaultAsync(x =>
            x.StockSymbol == sellCurrency.Symbol && x.Id == int.Parse(result.Data!));

        if (existingPortfolio != null)
        {

            existingPortfolio.Quantity -= sellCurrency.Quantity;
            if (sellCurrency.Symbol=="ons")
            {
                existingPortfolio.HistoricalProfit += (sellCurrency.Price*((await unitOfWork.Currencies.FirstOrDefaultAsync(x=>x.Name=="usd"))!).Sell-existingPortfolio.BuyPrice) * sellCurrency.Quantity;
            }
            else
            {
                existingPortfolio.HistoricalProfit +=(sellCurrency.Price-existingPortfolio.BuyPrice) * sellCurrency.Quantity;
            }
            
            if (existingPortfolio.Quantity ==0 )
            {
                existingPortfolio.BuyPrice = 0;
            }
        }

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    

    }


    
}