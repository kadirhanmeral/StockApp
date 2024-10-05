namespace App.Application.Features.Currencies.Buy;

public record BuyCurrencyRequest(string Symbol, decimal Price, int Quantity);
