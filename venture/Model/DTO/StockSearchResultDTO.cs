namespace stock.Model.DTO
{
    public class StockSearchResultDto
    {
        public int StockId { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal? CurrentPrice { get; set; }
    }
}