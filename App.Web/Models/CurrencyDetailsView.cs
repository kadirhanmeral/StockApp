namespace App.Web.Models;

public class CurrencyDetailsView
{
    public string Currency { get; set; }
    public string Type { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public decimal Change { get; set; }
    
    public string CurrencyCode { get; set; } 
}