namespace App.Web.Models;

public class PaginatedStockViewModel
{
    public IEnumerable<StockDetailsView> Stocks { get; set; } = default!;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }

    public CurrencyListView? Currencies { get; set; } // Para birimleri için eklenen özellik

}
