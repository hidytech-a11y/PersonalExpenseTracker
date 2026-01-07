using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Services;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BudgetStatus>>> GetAllBudgets()
        {
            var budgets = await _budgetService.GetAllBudgetsAsync();
            var result = new List<BudgetStatus>();

            foreach (var budget in budgets)
            {
                var spent = await _budgetService.GetSpentAmountAsync(
                    budget.Category, budget.Month, budget.Year);

                result.Add(new BudgetStatus
                {
                    Id = budget.Id,
                    Category = budget.Category,
                    MonthlyLimit = budget.MonthlyLimit,
                    Spent = spent,
                    Remaining = budget.MonthlyLimit - spent,
                    Percentage = budget.MonthlyLimit > 0
                        ? (spent / budget.MonthlyLimit) * 100
                        : 0,
                    Month = budget.Month,
                    Year = budget.Year
                });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            var existing = await _budgetService.GetBudgetAsync(
                dto.Category, dto.Month, dto.Year);

            if (existing != null)
            {
                return BadRequest(new { message = "Budget already exists for this category and month" });
            }

            var budget = new Budget
            {
                Category = dto.Category,
                MonthlyLimit = dto.MonthlyLimit,
                Month = dto.Month,
                Year = dto.Year
            };

            await _budgetService.AddBudgetAsync(budget);
            return Ok(budget);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto)
        {
            var success = await _budgetService.UpdateBudgetAsync(id, dto.MonthlyLimit);
            if (!success)
            {
                return NotFound(new { message = "Budget not found" });
            }
            return Ok(new { message = "Budget updated" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBudget(Guid id)
        {
            var success = await _budgetService.DeleteBudgetAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Budget not found" });
            }
            return Ok(new { message = "Budget deleted" });
        }
    }

    public class BudgetStatus
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal MonthlyLimit { get; set; }
        public decimal Spent { get; set; }
        public decimal Remaining { get; set; }
        public decimal Percentage { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class CreateBudgetDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class UpdateBudgetDto
    {
        public decimal MonthlyLimit { get; set; }
    }
}