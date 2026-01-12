using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.Models.DTOs
{
    public class CreateBudgetDto
    {
        [Required]
        public string Category { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Limit must be greater than 0")]
        public decimal MonthlyLimit { get; set; }

        public bool EnableRollover { get; set; }
    }
}