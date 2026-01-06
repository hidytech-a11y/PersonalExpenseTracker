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
        public ActionResult<List<Expense>> GetAllExpenses()
        {
            var expenses = _expenseService.GetAllExpenses();
            return Ok(expenses);
        }

        // GET: api/expenses/{id}
        [HttpGet("{id}")]
        public ActionResult<Expense> GetExpenseById(Guid id)
        {
            var expense = _expenseService.GetExpenseById(id);
            if (expense == null)
            {
                return NotFound(new { message = "Expense not found" });
            }
            return Ok(expense);
        }

        // GET: api/expenses/category/{category}
        [HttpGet("category/{category}")]
        public ActionResult<List<Expense>> GetExpensesByCategory(string category)
        {
            var expenses = _expenseService.GetExpensesByCategory(category);
            return Ok(expenses);
        }

        // GET: api/expenses/categories
        [HttpGet("categories")]
        public ActionResult<List<string>> GetAllCategories()
        {
            var categories = _expenseService.GetAllCategories();
            return Ok(categories);
        }

        // GET: api/expenses/total
        [HttpGet("total")]
        public ActionResult<object> GetTotalSpending()
        {
            var total = _expenseService.GetTotalSpending();
            var count = _expenseService.GetAllExpenses().Count;
            return Ok(new
            {
                totalAmount = total,
                expenseCount = count,
                average = count > 0 ? total / count : 0
            });
        }

        // POST: api/expenses
        [HttpPost]
        public ActionResult<Expense> CreateExpense([FromBody] CreateExpenseDto dto)
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

            _expenseService.AddExpense(expense);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateExpense(Guid id, [FromBody] UpdateExpenseDto dto)
        {
            var success = _expenseService.UpdateExpense(id, dto.Amount, dto.Category, dto.Description);
            if (!success)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var updatedExpense = _expenseService.GetExpenseById(id);
            return Ok(updatedExpense);
        }

        // DELETE: api/expenses/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteExpense(Guid id)
        {
            var success = _expenseService.DeleteExpense(id);
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