using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class TransactionHistory:BaseEntity<int>,IAuditEntity
{
    
    public int UserId { get; set; }
    
    public string StockSymbol { get; set; }
    
    public DateTime Time { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; }
    
    public TradeType TradeType { get; set; }


    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
public enum TradeType
{
    Buy = 1,
    Sell = 2
}