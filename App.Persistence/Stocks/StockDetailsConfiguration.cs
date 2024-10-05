using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Persistence.Stocks;

public class StockDetailsConfiguration:IEntityTypeConfiguration<StockDetails>
{
    public void Configure(EntityTypeBuilder<StockDetails> builder)
    {
        builder.Property(e => e.Price)
            .HasColumnType("decimal(18, 2)"); 

        builder.Property(e => e.PercentChange)
            .HasColumnType("decimal(18, 2)");
    }
}