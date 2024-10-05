namespace App.Application.Features.Buy;

public record BuyStockRequest(string Symbol, decimal Price, int Quantity);
