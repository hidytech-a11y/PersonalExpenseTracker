using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Models.DTOs;
using ExpenseTracker.API.Services;

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
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.Month == currentMonth && b.Year == currentYear)
                .ToListAsync();

            var budgetDtos = new List<BudgetDto>();

            foreach (var b in budgets)
            {
                var spent = await _context.Expenses
                    .Where(e => e.UserId == userId
                             && e.Category == b.Category
                             && e.Date.Month == currentMonth
                             && e.Date.Year == currentYear)
                    .SumAsync(e => e.Amount);

                // Use MonthlyLimit here instead of Amount
                double percentage = 0;
                if (b.MonthlyLimit > 0)
                {
                    percentage = (double)(spent / b.MonthlyLimit) * 100;
                }

                budgetDtos.Add(new BudgetDto
                {
                    Id = b.Id,
                    Category = b.Category,
                    MonthlyLimit = b.MonthlyLimit, // Map from Model to DTO
                    Month = b.Month,
                    Year = b.Year,
                    Spent = spent,
                    Percentage = percentage
                });
            }

            return Ok(budgetDtos);
        }

        // POST: api/Budgets
        [HttpPost]
        public async Task<ActionResult<Budget>> CreateBudget(Budget budget)
        {
            var userId = _authService.GetUserId();

            budget.UserId = userId;

            // Set defaults if not provided
            if (budget.Month == 0) budget.Month = DateTime.Now.Month;
            if (budget.Year == 0) budget.Year = DateTime.Now.Year;

            // Check for duplicates
            bool exists = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.Category == budget.Category &&
                b.Month == budget.Month &&
                b.Year == budget.Year);

            if (exists)
            {
                return BadRequest("A budget for this category and month already exists.");
            }

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudgets), new { id = budget.Id }, budget);
        }

        // DELETE: api/Budgets/{guid}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id) // Changed int to Guid
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