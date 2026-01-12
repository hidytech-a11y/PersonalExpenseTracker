using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models.DTOs;
using ExpenseTracker.API.Services;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly ExpenseDbContext _context;
        private readonly IAuthService _authService;

        public AnalyticsController(ExpenseDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var userId = _authService.GetUserId();
            var now = DateTime.UtcNow;

            // Time Ranges
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddSeconds(-1);
            var startOfTrend = now.AddDays(-30);
            var startOfSixMonths = startOfThisMonth.AddMonths(-5); // Go back 5 months + current

            // 1. Base Query
            var userExpenses = _context.Expenses.Where(e => e.UserId == userId);

            // 2. Headline Stats
            var thisMonthExpenses = userExpenses.Where(e => e.Date >= startOfThisMonth);
            var totalSpent = await thisMonthExpenses.SumAsync(e => e.Amount);
            var count = await thisMonthExpenses.CountAsync();

            // 3. Comparison Stats
            var lastMonthTotal = await userExpenses
                .Where(e => e.Date >= startOfLastMonth && e.Date <= endOfLastMonth)
                .SumAsync(e => e.Amount);

            double percentageChange = 0;
            if (lastMonthTotal > 0)
            {
                percentageChange = (double)((totalSpent - lastMonthTotal) / lastMonthTotal) * 100;
            }
            else if (totalSpent > 0)
            {
                percentageChange = 100;
            }

            var categoryStats = await thisMonthExpenses
                .GroupBy(e => e.Category)
                .Select(g => new { Name = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            var topCategory = categoryStats.FirstOrDefault()?.Name ?? "None";

            // 4. Chart Data: Category
            var categoryPoints = categoryStats
                .Select(x => new ChartDataPoint { Label = x.Name, Value = x.Total })
                .ToList();

            // 5. Chart Data: Daily Trend (30 Days)
            var rawTrendData = await userExpenses
                .Where(e => e.Date >= startOfTrend)
                .GroupBy(e => e.Date.Date)
                .Select(g => new { Date = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var trendStats = rawTrendData.Select(x => new ChartDataPoint
            {
                Label = x.Date.ToString("MM/dd"),
                Value = x.TotalAmount
            }).ToList();

            // 6. NEW: Chart Data: 6-Month Bar Chart
            var rawSixMonthData = await userExpenses
                .Where(e => e.Date >= startOfSixMonths)
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(e => e.Amount) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var sixMonthStats = rawSixMonthData.Select(x => new ChartDataPoint
            {
                // Format as "Jan", "Feb", etc.
                Label = new DateTime(x.Year, x.Month, 1).ToString("MMM"),
                Value = x.Total
            }).ToList();

            return Ok(new DashboardStatsDto
            {
                TotalSpent = totalSpent,
                TransactionCount = count,
                TotalSpentLastMonth = lastMonthTotal,
                PercentageChange = Math.Round(percentageChange, 1),
                TopCategory = topCategory,
                CategoryBreakdown = categoryPoints,
                MonthlyTrend = trendStats,
                SixMonthStats = sixMonthStats // Return new data
            });
        }
    }
}