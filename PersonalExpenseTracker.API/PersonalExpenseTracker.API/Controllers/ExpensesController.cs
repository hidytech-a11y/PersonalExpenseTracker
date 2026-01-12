using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

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
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return Guid.Parse(userIdClaim);
        }

        // GET: api/Expenses (Renamed to GetAllExpenses as requested)
        [HttpGet]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses(
            [FromQuery] string? search,
            [FromQuery] string? category,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetUserId();

            // 1. Fetch ALL expenses from the service
            var allExpenses = await _expenseService.GetAllExpensesAsync();

            // 2. Start Filtering (In Memory)
            // We filter by UserID first
            var query = allExpenses.Where(e => e.UserId == userId);

            // 3. Apply Search Filter (Description)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 4. Apply Category Filter
            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                query = query.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Apply Date Range Filter
            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Ensure we include the full end day (up to 23:59:59)
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(e => e.Date <= endOfDay);
            }

            // 6. Return sorted results
            return Ok(query.OrderByDescending(e => e.Date).ToList());
        }

        // GET: api/Expenses/{id}
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

        // GET: api/Expenses/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetAllCategories()
        {
            var categories = await _expenseService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Expenses/total
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

        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense([FromBody] CreateExpenseDto dto)
        {
            var userId = GetUserId();

            var expense = new Expense
            {
                Amount = dto.Amount,
                Category = dto.Category,
                Description = dto.Description,
                Date = dto.Date, // Ensure Date is captured from DTO
                UserId = userId
            };

            await _expenseService.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        // PUT: api/Expenses/{id}
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
                return NotFound(new { message = "Update failed" });
            }

            var updatedExpense = await _expenseService.GetExpenseByIdAsync(id);
            return Ok(updatedExpense);
        }

        // DELETE: api/Expenses/{id}
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
                return NotFound(new { message = "Delete failed" });
            }

            return Ok(new { message = "Expense deleted successfully" });
        }


        // GET: api/Expenses/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportExpenses()
        {
            var userId = GetUserId();
            var allExpenses = await _expenseService.GetAllExpensesAsync();

            // Get user's data sorted by date
            var userExpenses = allExpenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToList();

            // Build the CSV string
            var builder = new StringBuilder();

            // 1. Add Header Row
            builder.AppendLine("Date,Category,Description,Amount (NGN)");

            // 2. Add Data Rows
            foreach (var e in userExpenses)
            {
                // Sanitize description: If it contains a comma, wrap it in quotes to avoid breaking the CSV format
                var safeDescription = e.Description.Contains(",") ? $"\"{e.Description}\"" : e.Description;

                builder.AppendLine($"{e.Date:yyyy-MM-dd},{e.Category},{safeDescription},{e.Amount}");
            }

            // 3. Return as File
            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            var fileName = $"Expenses_{DateTime.Now:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }

    // DTOs
    public class CreateExpenseDto
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow; // Added Date support
    }

    public class UpdateExpenseDto
    {
        public decimal? Amount { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}