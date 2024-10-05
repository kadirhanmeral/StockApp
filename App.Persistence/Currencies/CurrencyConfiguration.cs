using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Persistence.Currencies;

public class CurrencyConfiguration:IEntityTypeConfiguration<Currency>
{
   
    

    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(e => e.Buy)
            .HasColumnType("decimal(18, 2)"); 

        builder.Property(e => e.Sell)
            .HasColumnType("decimal(18, 2)");
        builder.Property(e => e.ChangePercent)
            .HasColumnType("decimal(18, 2)");
    }
}