namespace stock.Model.DTO;

public class BuyStockDto
{
    public int UserId { get; set; }
    public int StockId { get; set; }
    public int Quantity { get; set; }
}