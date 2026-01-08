using ExpenseTracker.API.Models;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecurringExpensesController : ControllerBase
    {
        private readonly IRecurringExpenseService _service;

        public RecurringExpensesController(IRecurringExpenseService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<ActionResult<List<RecurringExpense>>> GetAll()
        {
            var userId = GetUserId();
            var recurring = await _service.GetAllAsync(userId);
            return Ok(recurring);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateRecurringDto dto)
        {
            var userId = GetUserId();
            var recurring = new RecurringExpense
            {
                Amount = dto.Amount,
                Category = dto.Category,
                Description = dto.Description,
                Frequency = dto.Frequency,
                DayOfMonth = dto.DayOfMonth,
                UserId = userId
            };

            await _service.AddAsync(recurring);
            return Ok(recurring);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateRecurringDto dto)
        {
            var userId = GetUserId();
            var existing = await _service.GetByIdAsync(id);

            if (existing == null || existing.UserId != userId)
            {
                return NotFound();
            }

            var updated = new RecurringExpense
            {
                Amount = dto.Amount,
                Category = dto.Category,
                Description = dto.Description,
                Frequency = dto.Frequency,
                DayOfMonth = dto.DayOfMonth,
                IsActive = dto.IsActive
            };

            await _service.UpdateAsync(id, updated);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var existing = await _service.GetByIdAsync(id);

            if (existing == null || existing.UserId != userId)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return Ok();
        }

        [HttpPost("process")]
        public async Task<ActionResult> ProcessRecurring()
        {
            await _service.ProcessRecurringExpensesAsync();
            return Ok(new { message = "Recurring expenses processed" });
        }
    }

    public class CreateRecurringDto
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Frequency { get; set; } = "Monthly";
        public int DayOfMonth { get; set; }
    }

    public class UpdateRecurringDto
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Frequency { get; set; } = "Monthly";
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
    }
}