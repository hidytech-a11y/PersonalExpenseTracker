using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Services
{
    public class RecurringExpenseRepository : IRecurringExpenseService
    {
        private readonly ExpenseDbContext _context;

        public RecurringExpenseRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecurringExpense>> GetAllAsync(Guid userId)
        {
            return await _context.RecurringExpenses
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Description)
                .ToListAsync();
        }

        public async Task<RecurringExpense?> GetByIdAsync(Guid id)
        {
            return await _context.RecurringExpenses.FindAsync(id);
        }

        public async Task AddAsync(RecurringExpense recurring)
        {
            _context.RecurringExpenses.Add(recurring);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, RecurringExpense recurring)
        {
            var existing = await _context.RecurringExpenses.FindAsync(id);
            if (existing == null)
                return false;

            existing.Amount = recurring.Amount;
            existing.Category = recurring.Category;
            existing.Description = recurring.Description;
            existing.Frequency = recurring.Frequency;
            existing.DayOfMonth = recurring.DayOfMonth;
            existing.IsActive = recurring.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var recurring = await _context.RecurringExpenses.FindAsync(id);
            if (recurring == null)
                return false;

            _context.RecurringExpenses.Remove(recurring);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ProcessRecurringExpensesAsync()
        {
            var today = DateTime.UtcNow.Date;
            var recurringExpenses = await _context.RecurringExpenses
                .Where(r => r.IsActive)
                .ToListAsync();

            foreach (var recurring in recurringExpenses)
            {
                bool shouldCreate = false;

                if (recurring.Frequency == "Monthly" && today.Day == recurring.DayOfMonth)
                {
                    if (recurring.LastProcessed == null ||
                        recurring.LastProcessed.Value.Month != today.Month)
                    {
                        shouldCreate = true;
                    }
                }

                if (shouldCreate)
                {
                    var expense = new Expense
                    {
                        Amount = recurring.Amount,
                        Category = recurring.Category,
                        Description = $"{recurring.Description} (Auto)",
                        Date = today,
                        UserId = recurring.UserId
                    };

                    _context.Expenses.Add(expense);
                    recurring.LastProcessed = today;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}