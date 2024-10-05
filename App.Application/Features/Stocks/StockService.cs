using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using App.Application.Contracts.Authorization;
using App.Application.Contracts.Persistence;
using App.Application.Features.Buy;
using App.Application.Features.Dto;
using App.Application.Features.Stocks.Dto;
using App.Domain.Entities;
using Newtonsoft.Json;
using YahooFinanceApi;

namespace App.Application.Features.Stocks;

public class StockService(IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService,
    HttpClient httpClient) : IStockService
{
    
    //get all stock list from hurriyet

    public async Task<ServiceResult<StockListDto>> GetStockListAsync()
    {
        var response = await httpClient.GetStringAsync("http://bigpara.hurriyet.com.tr/api/v1/hisse/list");
        var stockList = JsonConvert.DeserializeObject<StockListDto>(response);
        if (stockList == null)
        {
            return ServiceResult<StockListDto>.Fail("Stock List not found");
        }
        
        return ServiceResult<StockListDto>.Success(stockList);
    }
    
    //to use only for bist100 we check for bist100 stocks

    public async Task<ServiceResult> PushBist100DataToDatabase()
    {

        var stockList = await GetStockListAsync();

        foreach (var stock in stockList.Data!.Data)
        {
            if (Bist100.Codes.Contains(stock.Kod))
            {
                var newStock = new Stock
                {
                    Code = stock.Kod,
                    Name = stock.Ad,
                    Type = stock.Tip
                };
                await unitOfWork.StockRepository.AddAsync(newStock);
            }
        }

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
  
    }

    
    public async Task<ServiceResult> PushStockDetailsToDatabase()
    {
        try
        {
            var stocks = await unitOfWork.StockRepository.GetAllAsync();

            var symbols = stocks.Select(s => s.Code + ".IS").ToList();
            var symbolsString = string.Join(",", symbols);
            
            var stockList = stocks.Select(s => new 
            {
                Symbol = s.Code + ".IS", 
                Name = s.Name
            }).ToList();
            
            // Query Yahoo Finance API 
            var securities = await Yahoo.Symbols(symbolsString)
                .Fields(Field.Symbol, Field.RegularMarketPrice, Field.RegularMarketChangePercent, Field.RegularMarketTime)
                .QueryAsync();
            foreach (var symbol in stockList)
            {
                var stock = securities[symbol.Symbol];

                var price = Convert.ToDecimal(stock[Field.RegularMarketPrice]);
                var percentChange = Convert.ToDecimal(stock[Field.RegularMarketChangePercent]);
                var time = stock[Field.RegularMarketTime];

                var stockModel = new StockDetails()
                {
                    Symbol = symbol.Symbol,
                    Name = symbol.Name,
                    Price = price,
                    PercentChange = percentChange,
                    Time = UnixTimeToDateTime((long)time),
                };
                
                await unitOfWork.StocksDetails.AddAsync(stockModel);
            }

            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return ServiceResult.Fail("An error occurred");
        }
    }
    
    public async Task<ServiceResult> UpdateStockDetails()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var symbols = Bist100.Codes.Select(code => code + ".IS").ToList();
            var symbolsString = string.Join(",", symbols);
            
            
            var securities = await Yahoo.Symbols(symbolsString)
                .Fields(Field.Symbol, Field.RegularMarketPrice, Field.RegularMarketChangePercent, Field.RegularMarketTime)
                .QueryAsync();
            
            foreach (var symbol in symbols)
            {
                var existingStock = await unitOfWork.StocksDetails
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);
                
                var stock = securities[symbol];

                var price = Convert.ToDecimal(stock[Field.RegularMarketPrice]);
                var percentChange = Convert.ToDecimal(stock[Field.RegularMarketChangePercent]);
                var time = stock[Field.RegularMarketTime];

                if (existingStock == null)
                {
                    return ServiceResult.Fail("Stock Details not found");
                }
                else
                {
                    existingStock.Price = price;
                    existingStock.PercentChange = percentChange;
                    existingStock.Time = UnixTimeToDateTime((long)time);
                }
            }

            await unitOfWork.SaveChangesAsync();
            stopwatch.Stop();
        
            Console.WriteLine($"Method execution time: {stopwatch.Elapsed.TotalSeconds} seconds");

            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return ServiceResult.Fail("An error occurred");
        }
    }

    private DateTime UnixTimeToDateTime(long unixTime)
    {
        // Convert Unix timestamp to DateTime
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(unixTime).ToLocalTime();
    }
    
    public async Task<ServiceResult<List<StockDetails>>> GetStocksDataPaged(int page, int pageSize)
    {

        var stockDetails = await unitOfWork.StocksDetails.GetAllPagedAsync(page, pageSize);
        

        return ServiceResult<List<StockDetails>>.Success(stockDetails);
        
    }
    
    public async Task<ServiceResult<StockDetails>> GetStockDetails(string symbol)
    {

        
        var stock = await unitOfWork.StocksDetails.FirstOrDefaultAsync(s => s.Symbol == symbol);

        if (stock ==null)
        {
            return ServiceResult<StockDetails>.Fail("Stock not found",HttpStatusCode.NotFound);
        }
            
        var result = new StockDetails()
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            Name = stock.Name,
            Price = stock.Price,
            PercentChange = stock.PercentChange,
            Time = stock.Time
                
        };

        return ServiceResult<StockDetails>.Success(result);

    }

    public async Task<ServiceResult> BuyStock(BuyStockRequest buyStock)
    {
   
        var result =await authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);
        if (result.IsFail)
        {
            return ServiceResult.Fail(result.ErrorMessages!);
        }

        var existingStock = await unitOfWork.StocksDetails.FirstOrDefaultAsync(s => s.Symbol == buyStock.Symbol);
        if (existingStock == null)
        {
            return ServiceResult.Fail("Stock not found", HttpStatusCode.NotFound);
        }
        var transaction = new TransactionHistory
        {
            UserId = int.Parse(result.Data!),
            StockSymbol = buyStock.Symbol,
            Time = DateTime.Now,
            Price = buyStock.Price,
            Quantity = buyStock.Quantity,
            TradeType = TradeType.Buy

        };
        await unitOfWork.TransactionHistories.AddAsync(transaction);
        
        var existingPortfolio = await unitOfWork.Portfolios.FirstOrDefaultAsync(x =>
            x.StockSymbol == buyStock.Symbol && x.Id == int.Parse(result.Data!));
        
        if (existingPortfolio != null)
        {
            var totalPriceOfPortfolio = existingPortfolio.BuyPrice * existingPortfolio.Quantity;
            var totalPriceOfNewTransaction = buyStock.Quantity * buyStock.Price;
            existingPortfolio.Quantity += buyStock.Quantity;
            existingPortfolio.BuyPrice = (totalPriceOfPortfolio + totalPriceOfNewTransaction) /
                                         (existingPortfolio.Quantity);

        }
        else
        {
            var newStock = new Portfolio
            {
                Id = int.Parse(result.Data!),
                StockSymbol = buyStock.Symbol,
                BuyPrice = buyStock.Price,
                Quantity = buyStock.Quantity,
                Type = "Hisse"
            };
            await unitOfWork.Portfolios.AddAsync(newStock);
        }


        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    

    }
    
    public async Task<ServiceResult> SellStock(BuyStockRequest sellStock)
    {
   
        var result =await authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);
        var claim = authorizationService.FindFirstValue(ClaimTypes.NameIdentifier);

        if (result.IsFail)
        {
            return ServiceResult.Fail(result.ErrorMessages!);
        }

        var existingStock = await unitOfWork.StocksDetails.FirstOrDefaultAsync(s => s.Symbol == sellStock.Symbol);
        if (existingStock == null)
        {
            return ServiceResult.Fail("Stock not found", HttpStatusCode.NotFound);
        }

        var userId = int.Parse(claim.Result.Data!);

        var dataList = await unitOfWork.Portfolios.GetPortfolioByUserIdAndSymbolAsync(userId, sellStock.Symbol);

        if (dataList==null)
        {
            return ServiceResult.Fail("You don't have any stock available to sell");

        }

        if (sellStock.Quantity>dataList.Quantity)
        {
            return ServiceResult.Fail("You can't sell more than you have");

        }
        
        

        var transaction = new TransactionHistory
        {
            UserId = int.Parse(result.Data!),
            StockSymbol = sellStock.Symbol,
            Time = DateTime.Now,
            Price = sellStock.Price,
            Quantity = sellStock.Quantity,
            TradeType = TradeType.Sell

        };
        await unitOfWork.TransactionHistories.AddAsync(transaction);
        
        var existingPortfolio = await unitOfWork.Portfolios.FirstOrDefaultAsync(x =>
            x.StockSymbol == sellStock.Symbol && x.Id == int.Parse(result.Data!));

        if (existingPortfolio != null)
        {

            existingPortfolio.Quantity -= sellStock.Quantity;
            existingPortfolio.HistoricalProfit +=(sellStock.Price-existingPortfolio.BuyPrice) * sellStock.Quantity;
            if (existingPortfolio.Quantity ==0 )
            {
                existingPortfolio.BuyPrice = 0;
            }
            
        }

       

        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    

    }






}