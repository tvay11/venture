using System.ComponentModel.DataAnnotations;

namespace stock.Model.DTO
{
    public class StockCreateDto
    {
        [Required]
        [StringLength(10)] 
        public string Symbol { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public List<StockPriceDto> Prices { get; set; }
    }
}