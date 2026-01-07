using System.ComponentModel.DataAnnotations;


namespace ExpenseTracker.API.Models
{
    public class RecurringExpense
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }


        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;


        [Required]
        public string Frequency { get; set; } = "Monthly"; 

        [Required]
        public int DayOfMonth { get; set; } // 1-31 for monthly

        public DateTime? LastProcessed { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public Guid UserId { get; set; }

        public RecurringExpense()
        {
            Id = Guid.NewGuid();
        }
    }
}
