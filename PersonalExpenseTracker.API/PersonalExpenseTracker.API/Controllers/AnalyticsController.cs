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
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfTrend = now.AddDays(-30);

            // 1. Base Query
            var userExpenses = _context.Expenses.Where(e => e.UserId == userId);

            // 2. Headline Stats (Current Month)
            var monthExpenses = userExpenses.Where(e => e.Date >= startOfMonth);

            var totalSpent = await monthExpenses.SumAsync(e => e.Amount);
            var count = await monthExpenses.CountAsync();

            // 3. Category Breakdown
            var categoryStats = await monthExpenses
                .GroupBy(e => e.Category)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = g.Sum(e => e.Amount)
                })
                .ToListAsync();

            // 4. Trend (Last 30 Days) - FIXED: Fetch data first, format later
            var rawTrendData = await userExpenses
                .Where(e => e.Date >= startOfTrend)
                .GroupBy(e => e.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Perform the formatting in memory (Client-Side Evaluation)
            var trendStats = rawTrendData.Select(x => new ChartDataPoint
            {
                Label = x.Date.ToString("MM/dd"),
                Value = x.TotalAmount
            }).ToList();

            // 5. Assemble DTO
            var stats = new DashboardStatsDto
            {
                TotalSpent = totalSpent,
                TransactionCount = count,
                CategoryBreakdown = categoryStats,
                MonthlyTrend = trendStats
            };

            return Ok(stats);
        }
    }
}