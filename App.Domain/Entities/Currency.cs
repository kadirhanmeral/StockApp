using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class Currency:BaseEntity<int>,IAuditEntity
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } =  default!;
    public decimal Buy { get; set; } 
    public decimal Sell { get; set; }
    public decimal ChangePercent { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}