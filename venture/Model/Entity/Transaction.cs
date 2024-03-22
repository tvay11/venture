namespace stock.Model.Entity;

public class Transaction
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
}