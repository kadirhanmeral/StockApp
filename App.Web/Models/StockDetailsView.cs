namespace App.Web.Models;

public class StockDetailsView
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal PercentChange { get; set; }
    public DateTime Time { get; set; }

}