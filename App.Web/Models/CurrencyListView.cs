namespace App.Web.Models;

public class CurrencyListView
{
    public DateTime UpdateDate { get; set; }
    public List<CurrencyView> Data { get; set; }
}

public class CurrencyView
{
    public string Currency { get; set; }
    public string Type { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public decimal Change { get; set; }
    
    public string CurrencyCode { get; set; } 
}