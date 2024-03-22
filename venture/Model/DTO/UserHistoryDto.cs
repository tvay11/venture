namespace stock.Model.DTO;

public class UserHistoryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal NetWorth { get; set; }
}
