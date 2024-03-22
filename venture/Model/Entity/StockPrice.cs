using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stock.Model.Entity
{
    public class StockPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        public int StockId { get; set; } 
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        
    }
}