namespace App.Web.Models;


public class BuyStockView
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}