    namespace stock.Model.DTO

{
    public class StockResponseDto
    {
        public int StockId { get; set; } 
        public string Symbol { get; set; }
        public string Name { get; set; }
        
        public DateTime CurrentDate { get; set; }
        public List<StockPriceDto> Prices { get; set; }
        
    }

}