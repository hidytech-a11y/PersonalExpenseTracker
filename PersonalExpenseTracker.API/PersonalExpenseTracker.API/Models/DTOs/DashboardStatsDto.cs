namespace ExpenseTracker.API.Models.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalSpent { get; set; }
        public int TransactionCount { get; set; }

        // Data for the Doughnut Chart
        public List<ChartDataPoint> CategoryBreakdown { get; set; } = new();

        // Data for the Line Chart (Last 30 Days)
        public List<ChartDataPoint> MonthlyTrend { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}