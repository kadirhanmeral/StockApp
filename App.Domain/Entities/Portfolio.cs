using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class Portfolio:BaseEntity<int>,IAuditEntity
{
    public string StockSymbol { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal BuyPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal HistoricalProfit { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
