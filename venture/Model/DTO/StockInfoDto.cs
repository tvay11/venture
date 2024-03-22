namespace stock.Model.DTO;

public class StockInfoDto
{
    public int StockId { get; set; } 
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal CurrentPrice { get; set; }
    public int Quantity { get; set; }
    public DateTime CurrentDate { get; set; }
}