using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class StockDetails:BaseEntity<int>,IAuditEntity
{
    
    public string Symbol { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PercentChange { get; set; }
    
    public string Type { get; set; }

    public DateTime Time { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}