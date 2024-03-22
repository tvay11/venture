    using System.ComponentModel.DataAnnotations;

    namespace stock.Model.Entity
    {

        public class StockHolding
        {
            public int Id { get; set; } 
            public int UserId { get; set; } 
            public virtual UserProfile User { get; set; } 
            public int StockId { get; set; } 
            public virtual Stock Stock { get; set; } 
            public int Quantity { get; set; }
        }

    }
