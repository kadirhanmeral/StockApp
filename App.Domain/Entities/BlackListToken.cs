using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class BlackListToken:BaseEntity<int>,IAuditEntity
{
    public string? Token { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}