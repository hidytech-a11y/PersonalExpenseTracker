using ExpenseTracker.Api.Models;
using System.Text.Json;

namespace ExpenseTracker.Api.Services
{
    public class ExpenseService : IExpenseService
    {
        private List<Expense> _expenses;
        private readonly string _filePath = "expenses.json";
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public ExpenseService()
        {
            _expenses = new List<Expense>();
            LoadExpenses();
        }

        public void AddExpense(Expense expense)
        {
            _expenses.Add(expense);
            SaveExpenses();
        }

        public List<Expense> GetAllExpenses()
        {
            return _expenses.OrderByDescending(e => e.Date).ToList();
        }

        public Expense? GetExpenseById(Guid id)
        {
            return _expenses.FirstOrDefault(e => e.Id == id);
        }

        public decimal GetTotalSpending()
        {
            return _expenses.Sum(e => e.Amount);
        }

        public bool DeleteExpense(Guid id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _expenses.Remove(expense);
                SaveExpenses();
                return true;
            }
            return false;
        }

        public bool UpdateExpense(Guid id, decimal? amount, string? category, string? description)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                if (amount.HasValue)
                    expense.Amount = amount.Value;

                if (!string.IsNullOrWhiteSpace(category))
                    expense.Category = category;

                if (!string.IsNullOrWhiteSpace(description))
                    expense.Description = description;

                SaveExpenses();
                return true;
            }
            return false;
        }

        public List<Expense> GetExpensesByCategory(string category)
        {
            return _expenses
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.Date)
                .ToList();
        }

        public List<string> GetAllCategories()
        {
            return _expenses
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        private void SaveExpenses()
        {
            var json = JsonSerializer.Serialize(_expenses, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }

        private void LoadExpenses()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _expenses = JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
            }
        }

        void IExpenseService.SaveExpenses()
        {
            SaveExpenses();
        }

        void IExpenseService.LoadExpenses()
        {
            LoadExpenses();
        }
    }
}