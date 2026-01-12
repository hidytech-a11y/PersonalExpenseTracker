namespace ExpenseTracker.API.Models.DTOs
{
    public class BudgetDto
    {
        public Guid Id { get; set; }             // Changed from int to Guid
        public string Category { get; set; } = string.Empty;
        public decimal MonthlyLimit { get; set; } // Renamed from Amount
        public decimal Spent { get; set; }
        public double Percentage { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }


        public decimal RolloverAmount { get; set; } // New property for rollover amount
        public decimal EffectiveLimit { get; set; } // New property for effective limit
        public bool EnableRollover { get; set; } // New property to indicate if rollover is enabled
    }
}