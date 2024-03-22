using System.ComponentModel.DataAnnotations;

namespace stock.Model.Entity
{
    public class Stock
    {
        [Key] public int Id { get; set; }

        public string Symbol { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<StockPrice> StockPrices { get; set; }
        public virtual ICollection<StockHolding> StockHoldings { get; set; }
    }
}