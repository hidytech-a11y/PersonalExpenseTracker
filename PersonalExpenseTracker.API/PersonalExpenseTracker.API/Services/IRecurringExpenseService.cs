using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Services
{
    public interface IRecurringExpenseService
    {
        Task<List<RecurringExpense>> GetAllAsync(Guid userId);
        Task<RecurringExpense?> GetByIdAsync(Guid id);
        Task AddAsync(RecurringExpense recurring);
        Task<bool> UpdateAsync(Guid id, RecurringExpense recurring);
        Task<bool> DeleteAsync(Guid id);
        Task ProcessRecurringExpensesAsync();
    }
}
