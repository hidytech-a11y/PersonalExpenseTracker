using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.Models.DTOs;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly ExpenseDbContext _context;
        private readonly IAuthService _authService;

        public BudgetsController(ExpenseDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets()
        {
            var userId = _authService.GetUserId();
            var now = DateTime.UtcNow;

            // Define time windows
            var currentMonth = now.Month;
            var currentYear = now.Year;

            var startOfThisMonth = new DateTime(currentYear, currentMonth, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddSeconds(-1);

            // Fetch User's Budgets
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var budgetDtos = new List<BudgetDto>();

            foreach (var b in budgets)
            {
                // 1. Calculate Spent This Month
                var spentThisMonth = await _context.Expenses
                    .Where(e => e.UserId == userId &&
                                e.Category == b.Category &&
                                e.Date >= startOfThisMonth)
                    .SumAsync(e => e.Amount);

                // 2. Calculate Rollover (If Enabled)
                decimal rolloverAmount = 0;
                if (b.EnableRollover)
                {
                    var spentLastMonth = await _context.Expenses
                        .Where(e => e.UserId == userId &&
                                e.Category == b.Category &&
                                e.Date >= startOfLastMonth && e.Date <= endOfLastMonth)
                        .SumAsync(e => e.Amount);

                    // If spent less than limit, carry over the remainder
                    if (spentLastMonth < b.MonthlyLimit)
                    {
                        rolloverAmount = b.MonthlyLimit - spentLastMonth;
                    }
                }

                // 3. Calculate Totals
                decimal effectiveLimit = b.MonthlyLimit + rolloverAmount;
                double percentage = 0;
                if (effectiveLimit > 0)
                {
                    percentage = (double)(spentThisMonth / effectiveLimit) * 100;
                }

                budgetDtos.Add(new BudgetDto
                {
                    Id = b.Id,
                    Category = b.Category,
                    MonthlyLimit = b.MonthlyLimit,
                    Spent = spentThisMonth,
                    RolloverAmount = rolloverAmount,
                    EffectiveLimit = effectiveLimit,
                    Percentage = percentage,
                    EnableRollover = b.EnableRollover
                });
            }

            return Ok(budgetDtos);
        }

        // POST: api/Budgets
        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            var userId = _authService.GetUserId();

            // Check for duplicates
            bool exists = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.Category == dto.Category);

            if (exists)
            {
                return BadRequest("A budget for this category already exists.");
            }

            var budget = new Budget
            {
                UserId = userId,
                Category = dto.Category,
                MonthlyLimit = dto.MonthlyLimit,
                EnableRollover = dto.EnableRollover,
                // Defaulting Month/Year isn't strictly necessary if budgets are persistent across months, 
                // but we keep them for record if your DB schema requires them.
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudgets), new { id = budget.Id }, budget);
        }

        // PUT: api/Budgets/{id} (NEW FEATURE)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto)
        {
            var userId = _authService.GetUserId();

            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            if (budget.UserId != userId)
            {
                return Unauthorized();
            }

            // Update fields
            budget.Category = dto.Category;
            budget.MonthlyLimit = dto.MonthlyLimit;
            budget.EnableRollover = dto.EnableRollover;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Budgets.Any(b => b.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(budget);
        }

        // DELETE: api/Budgets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            var userId = _authService.GetUserId();

            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            if (budget.UserId != userId)
            {
                return Unauthorized();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}