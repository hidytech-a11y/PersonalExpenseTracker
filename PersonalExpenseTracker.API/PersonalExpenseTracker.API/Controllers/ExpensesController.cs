using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: api/expenses
        [HttpGet]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }

        // GET: api/expenses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(Guid id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = "Expense not found" });
            }
            return Ok(expense);
        }

        // GET: api/expenses/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<Expense>>> GetExpensesByCategory(string category)
        {
            var expenses = await _expenseService.GetExpensesByCategoryAsync(category);
            return Ok(expenses);
        }

        // GET: api/expenses/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetAllCategories()
        {
            var categories = await _expenseService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/expenses/total
        [HttpGet("total")]
        public async Task<ActionResult<object>> GetTotalSpending()
        {
            var total = await _expenseService.GetTotalSpendingAsync();
            var expenses = await _expenseService.GetAllExpensesAsync();
            var count = expenses.Count;

            return Ok(new
            {
                totalAmount = total,
                expenseCount = count,
                average = count > 0 ? total / count : 0
            });
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expense = new Expense
            {
                Amount = dto.Amount,
                Category = dto.Category,
                Description = dto.Description
            };

            await _expenseService.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto)
        {
            var success = await _expenseService.UpdateExpenseAsync(id, dto.Amount, dto.Category, dto.Description);
            if (!success)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var updatedExpense = await _expenseService.GetExpenseByIdAsync(id);
            return Ok(updatedExpense);
        }

        // DELETE: api/expenses/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExpense(Guid id)
        {
            var success = await _expenseService.DeleteExpenseAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Expense not found" });
            }
            return Ok(new { message = "Expense deleted successfully" });
        }
    }

    // DTOs (Data Transfer Objects)
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