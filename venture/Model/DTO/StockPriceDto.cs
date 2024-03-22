using System.ComponentModel.DataAnnotations;

namespace stock.Model.DTO
{

    public class StockPriceDto
    {
        [Required] public DateTime Date { get; set; }

        [Range(0, 1000000)]
        public decimal Price { get; set; }
    }
}