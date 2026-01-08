using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses()
        {
            var userId = GetUserId();
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses.Where(e => e.UserId == userId).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(Guid id)
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            if (expense == null || expense.UserId != userId)
            {
                return NotFound(new { message = "Expense not found" });
            }

            return Ok(expense);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<Expense>>> GetExpensesByCategory(string category)
        {
            var userId = GetUserId();
            var expenses = await _expenseService.GetExpensesByCategoryAsync(category);
            return Ok(expenses.Where(e => e.UserId == userId).ToList());
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetAllCategories()
        {
            var userId = GetUserId();
            var categories = await _expenseService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("total")]
        public async Task<ActionResult<object>> GetTotalSpending()
        {
            var userId = GetUserId();
            var allExpenses = await _expenseService.GetAllExpensesAsync();
            var userExpenses = allExpenses.Where(e => e.UserId == userId).ToList();

            var total = userExpenses.Sum(e => e.Amount);
            var count = userExpenses.Count;

            return Ok(new
            {
                totalAmount = total,
                expenseCount = count,
                average = count > 0 ? total / count : 0
            });
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseDto dto)
        {
            var userId = GetUserId();

            var expense = new Expense
            {
                Amount = dto.Amount,
                Category = dto.Category,
                Description = dto.Description,
                UserId = userId
            };

            await _expenseService.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto)
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            if (expense == null || expense.UserId != userId)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var success = await _expenseService.UpdateExpenseAsync(id, dto.Amount, dto.Category, dto.Description);
            if (!success)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var updatedExpense = await _expenseService.GetExpenseByIdAsync(id);
            return Ok(updatedExpense);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExpense(Guid id)
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            if (expense == null || expense.UserId != userId)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var success = await _expenseService.DeleteExpenseAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Expense not found" });
            }

            return Ok(new { message = "Expense deleted successfully" });
        }

        
        [HttpGet("daterange")]
        public async Task<ActionResult<List<Expense>>> GetExpensesByDateRange(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetUserId();
            var allExpenses = await _expenseService.GetAllExpensesAsync();
            var userExpenses = allExpenses.Where(e => e.UserId == userId);

            if (startDate.HasValue)
            {
                userExpenses = userExpenses.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Include the entire end date
                var endOfDay = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                userExpenses = userExpenses.Where(e => e.Date <= endOfDay);
            }

            return Ok(userExpenses.OrderByDescending(e => e.Date).ToList());
        }
    }

    public class CreateExpenseDto
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateExpenseDto
    {
        public decimal? Amount { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}