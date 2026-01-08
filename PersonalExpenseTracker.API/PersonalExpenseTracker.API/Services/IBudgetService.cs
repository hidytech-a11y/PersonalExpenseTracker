using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Services
{
    public interface IBudgetService
    {
        Task<List<Budget>> GetAllBudgetsAsync();
        Task<Budget?> GetBudgetAsync(string category, int month, int year, Guid userId);
        Task<Budget?> GetBudgetByIdAsync(Guid id);
        Task AddBudgetAsync(Budget budget);
        Task<bool> UpdateBudgetAsync(Guid id, decimal monthlyLimit);
        Task<bool> DeleteBudgetAsync(Guid id);
        Task<decimal> GetSpentAmountAsync(string category, int month, int year, Guid userId);
    }
}