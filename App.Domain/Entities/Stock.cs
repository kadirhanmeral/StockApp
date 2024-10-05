using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class Stock:BaseEntity<int>,IAuditEntity
{
    public string Code { get; set; }
    
    public string Name { get; set; }
    public string Type { get; set; }


    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    
}