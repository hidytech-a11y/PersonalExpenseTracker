namespace ExpenseTracker.API.Models.DTOs
{
    public class UpdateBudgetDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal MonthlyLimit { get; set; }
        public bool EnableRollover { get; set; }
    }
}