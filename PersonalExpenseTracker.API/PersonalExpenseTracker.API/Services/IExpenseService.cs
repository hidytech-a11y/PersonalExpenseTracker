using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Services
{
    public interface IExpenseService
    {
        Task AddExpenseAsync(Expense expense);
        Task<List<Expense>> GetAllExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(Guid id);
        Task<decimal> GetTotalSpendingAsync();
        Task<bool> DeleteExpenseAsync(Guid id);
        Task<bool> UpdateExpenseAsync(Guid id, decimal? amount, string? category, string? description);
        Task<List<Expense>> GetExpensesByCategoryAsync(string category);
        Task<List<string>> GetAllCategoriesAsync();
    }
}