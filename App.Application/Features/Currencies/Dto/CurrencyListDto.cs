using App.Domain.Entities;

namespace App.Application.Features.Currencies.Dto;

public class CurrencyListDto
{
    public DateTime UpdateDate { get; set; }
    public List<CurrencyDto> Data { get; set; }
}

