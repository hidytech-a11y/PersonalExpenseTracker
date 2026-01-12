namespace ExpenseTracker.API.Models.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalSpent { get; set; }
        public int TransactionCount { get; set; }

        // Comparison / Insight Fields
        public decimal TotalSpentLastMonth { get; set; }
        public double PercentageChange { get; set; }
        public string TopCategory { get; set; } = "None";

        // Lists for Charts
        public List<ChartDataPoint> CategoryBreakdown { get; set; } = new();
        public List<ChartDataPoint> MonthlyTrend { get; set; } = new();

        //New: 6-month Comparison Data
        public List<ChartDataPoint> SixMonthStats { get; set; } = new(); 
    }

    // This is the missing class causing the error
    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}