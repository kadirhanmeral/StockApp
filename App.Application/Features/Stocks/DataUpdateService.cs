using App.Application.Features.Currencies;
using Microsoft.Extensions.DependencyInjection;
namespace App.Application.Features.Stocks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class DataUpdateService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // 1-minute interval

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
            var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

            await currencyService.UpdateCurrenciesInDatabaseAsync();
            await stockService.UpdateStockDetails();
       
            await Task.Delay(_interval, stoppingToken); // Wait for the interval
        }
    }
    
    
}