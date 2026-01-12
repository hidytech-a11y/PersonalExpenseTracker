using System.ComponentModel.DataAnnotations;


namespace ExpenseTracker.API.Models
{
    public class Budget
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal MonthlyLimit { get; set; }


        [Required]
        public int Month { get; set; }  // 1-12

        [Required]
        public int Year { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Budget()
        {
            Id = Guid.NewGuid();
            Month = DateTime.UtcNow.Month;
            Year = DateTime.UtcNow.Year;

        }


        // New property to track amount spent
        public bool EnableRollover { get; set; } = false;
    }
}
