namespace App.Application.Features.Users.Profile;

public class GetProfileResponse
{
    public string? FullName { get; set; }

    public string? Email { get; set; }
    

    public decimal HistoricalStockProfit  { get; set; }
    
    public decimal CurrentStockProfit  { get; set; }
    public decimal TotalStockValue  { get; set; }

    
    public decimal TotalStockProfit  { get; set; }    
    public List<PortfolioDto> Data { get; set; }
    
    public List<TransactionDto> Transactions { get; set; }
}

public class PortfolioDto
{
    public string StockSymbol { get; set; }
    
    public decimal BuyPrice { get; set; }
    public int Quantity { get; set; }
    
    public decimal CurrentPrice { get; set; }

    public decimal HistoricalStockProfit  { get; set; }

    public decimal StockProfit  { get; set; }

    public decimal StockValue { get; set; }

    
}

public class TransactionDto
{
    public string StockSymbol { get; set; }
    
    public DateTime Time { get; set; }
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }

    public string TradeType  { get; set; }
    
}
