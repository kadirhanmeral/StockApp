namespace App.Application.Features.Stocks.Dto;
public class StockListDto
{
    public string Code { get; set; }
    public List<StockDto> Data { get; set; }
}

public class StockDto
{
    public int Id { get; set; }
    public string Kod { get; set; } 
    public string Ad { get; set; } 
    public string Tip { get; set; } 
}

