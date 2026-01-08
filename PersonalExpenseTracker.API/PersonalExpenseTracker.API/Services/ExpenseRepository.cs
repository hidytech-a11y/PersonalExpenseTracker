using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Services
{
    public class ExpenseRepository : IExpenseService
    {
        private readonly ExpenseDbContext _context;

        public ExpenseRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseByIdAsync(Guid id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<decimal> GetTotalSpendingAsync()
        {
            return await _context.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0;
        }

        public async Task<bool> DeleteExpenseAsync(Guid id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateExpenseAsync(Guid id, decimal? amount, string? category, string? description)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return false;

            if (amount.HasValue)
                expense.Amount = amount.Value;

            if (!string.IsNullOrWhiteSpace(category))
                expense.Category = category;

            if (!string.IsNullOrWhiteSpace(description))
                expense.Description = description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Expense>> GetExpensesByCategoryAsync(string category)
        {
            return await _context.Expenses
                .Where(e => e.Category.ToLower() == category.ToLower())
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _context.Expenses
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}