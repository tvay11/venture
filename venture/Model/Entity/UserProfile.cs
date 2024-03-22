using System.ComponentModel.DataAnnotations;
namespace stock.Model.Entity
{
    public class UserProfile
    {
        [Key] public int UserId { get; set; } = 1;
        public decimal Cash { get; set; } = 10000;
        public decimal NetWorth { get; set; } = 10000;
        public DateTime CurrentDate { get; set; } = DateTime.Today;

        public virtual ICollection<StockHolding> StockHoldings { get; set; }
    }
}