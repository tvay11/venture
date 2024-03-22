    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace stock.Model.Entity;

    public class UserHistory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserProfile")]
        public int UserId { get; set; }
        public virtual UserProfile UserProfile { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetWorth { get; set; }
    }