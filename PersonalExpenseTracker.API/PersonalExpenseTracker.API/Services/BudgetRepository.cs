using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services
{
    public class BudgetRepository : IBudgetService
    {
        private readonly ExpenseDbContext _context;

        public BudgetRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllBudgetsAsync()
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            return await _context.Budgets
                .Where(b => b.Month == currentMonth && b.Year == currentYear)
                .ToListAsync();
        }

        public async Task<Budget?> GetBudgetAsync(string category, int month, int year, Guid userId)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b =>
                    b.Category == category &&
                    b.Month == month &&
                    b.Year == year &&
                    b.UserId == userId);
        }

        public async Task<Budget?> GetBudgetByIdAsync(Guid id)
        {
            return await _context.Budgets.FindAsync(id);
        }

        public async Task AddBudgetAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateBudgetAsync(Guid id, decimal monthlyLimit)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
                return false;

            budget.MonthlyLimit = monthlyLimit;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBudgetAsync(Guid id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
                return false;

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetSpentAmountAsync(string category, int month, int year, Guid userId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            return await _context.Expenses
                .Where(e =>
                    e.Category == category &&
                    e.Date >= startDate &&
                    e.Date < endDate &&
                    e.UserId == userId)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;
        }
    }
}