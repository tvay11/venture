namespace stock.Model.DTO

{
    public class UserProfileDTO
    {
        public int UserId { get; set; }
        public decimal Cash { get; set; }
        public decimal NetWorth { get; set; }
        public DateTime CurrentDate { get; set; } 
    }
}